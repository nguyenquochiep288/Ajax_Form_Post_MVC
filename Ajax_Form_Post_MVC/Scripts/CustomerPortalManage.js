(function () {
    'use strict';
    var root = document.getElementById('portal-manage');
    if (!root) return;
    var status = document.getElementById('manage-status'), issue = document.getElementById('issue-link'), revoke = document.getElementById('revoke-link');
    var active = false, activeCustomer = false;
    var historyPage = 1, historyVersion = 0;
    function get(obj, key) { return obj[key] !== undefined ? obj[key] : obj[key.charAt(0).toUpperCase() + key.slice(1)]; }
    async function request(url, body) {
        var response = await fetch(url, { method: body ? 'POST' : 'GET', credentials: 'same-origin', cache: 'no-store', body: body });
        var data;
        try { data = await response.json(); } catch (_) { throw new Error('Chưa thể kết nối dữ liệu.'); }
        if (!response.ok) throw new Error(get(data, 'message') || 'Không thể thay đổi link.');
        return data;
    }
    function form() {
        var data = new FormData();
        data.append('id', root.dataset.id);
        data.append('__RequestVerificationToken', root.querySelector('[name="__RequestVerificationToken"]').value);
        return data;
    }
    function busy(value) { issue.disabled = value || !activeCustomer; revoke.disabled = value || !active; }
    async function refresh() {
        var data = await request(root.dataset.statusUrl + '?id=' + encodeURIComponent(root.dataset.id));
        var customer = get(data, 'customer');
        document.getElementById('manage-customer').textContent = (customer.name || customer.NAME || '') + ' · ' + (customer.ma || customer.MA || '');
        activeCustomer = Boolean(customer.isactive !== undefined ? customer.isactive : customer.ISACTIVE);
        active = Boolean(get(data, 'active'));
        var expiry = get(data, 'expiresUtc');
        status.textContent = !activeCustomer ? 'Khách hàng đã ngừng hoạt động.' : active ? 'Link đang hoạt động' + (expiry ? ' · Hết hạn: ' + new Date(expiry + (/[zZ]|[+-]\d\d:\d\d$/.test(expiry) ? '' : 'Z')).toLocaleString('vi-VN') : ' · Không giới hạn thời gian') : 'Chưa có link đang hoạt động.';
        var token = get(data, 'token');
        document.getElementById('legacy-link-note').hidden = !active || Boolean(token);
        document.getElementById('issued-link').hidden = !active || !token;
        if (active && token) {
            var url = new URL(root.dataset.portalBase + token, window.location.origin).href;
            document.getElementById('portal-link').value = url;
            document.getElementById('open-link').href = url;
        } else {
            document.getElementById('portal-link').value = '';
            document.getElementById('open-link').removeAttribute('href');
        }
        busy(false);
        loadHistory(1);
    }
    async function loadHistory(page) {
        var version = ++historyVersion, message = document.getElementById('history-status'), container = document.getElementById('history-content');
        var previous = document.getElementById('history-prev'), next = document.getElementById('history-next');
        previous.disabled = next.disabled = true; message.textContent = 'Đang tải nhật ký…'; container.replaceChildren();
        function element(tag, text) { var node = document.createElement(tag); if (text !== undefined) node.textContent = text; return node; }
        function time(value) { if (!value) return 'Không giới hạn'; return new Date(value + (/[zZ]|[+-]\d\d:\d\d$/.test(value) ? '' : 'Z')).toLocaleString('vi-VN'); }
        try {
            var data = await request(root.dataset.historyUrl + '?id=' + encodeURIComponent(root.dataset.id) + '&page=' + page);
            if (version !== historyVersion) return;
            var rows = get(data, 'rows'), total = get(data, 'total'); historyPage = get(data, 'page');
            var pages = Math.max(1, Math.ceil(total / get(data, 'pageSize')));
            if (!rows.length) message.textContent = 'Chưa có thao tác tạo, cấp lại hoặc thu hồi link được ghi nhận.';
            else {
                message.textContent = total + ' thao tác · Hiển thị theo giờ thiết bị';
                var table = element('table'), header = element('thead'), headerRow = element('tr'), body = element('tbody');
                ['Thời điểm', 'Thao tác', 'Nhân viên', 'Khách hàng', 'Thời hạn link'].forEach(function (label) { var cell = element('th', label); cell.scope = 'col'; headerRow.appendChild(cell); }); header.appendChild(headerRow);
                rows.forEach(function (item) {
                    var row = element('tr'), action = get(item, 'action'), labels = { created: 'Tạo link', reissued: 'Cấp lại link', revoked: 'Thu hồi link' };
                    var expiry = time(get(item, 'expiresUtc')), old = get(item, 'previousExpiresUtc');
                    if (action === 'reissued') expiry += ' · Trước đó: ' + time(old);
                    [time(get(item, 'occurredUtc')), labels[action] || action, get(item, 'actorDisplayName') + ' (' + get(item, 'actorUsername') + ')', get(item, 'customerName') + ' · ' + get(item, 'customerCode'), expiry].forEach(function (value) { row.appendChild(element('td', value)); }); body.appendChild(row);
                }); table.append(header, body); container.appendChild(table);
            }
            document.getElementById('history-page').textContent = 'Trang ' + historyPage + '/' + pages;
            previous.disabled = historyPage <= 1; next.disabled = historyPage >= pages;
        } catch (error) { if (version === historyVersion) { message.textContent = error.message; document.getElementById('history-page').textContent = ''; } }
    }
    document.getElementById('history-prev').addEventListener('click', function () { loadHistory(historyPage - 1); });
    document.getElementById('history-next').addEventListener('click', function () { loadHistory(historyPage + 1); });
    document.getElementById('link-form').addEventListener('submit', async function (event) {
        event.preventDefault();
        if (active && !window.confirm('Cấp lại link sẽ vô hiệu hóa link cũ. Tiếp tục?')) return;
        busy(true);
        try {
            var body = form(); body.append('validDays', document.getElementById('valid-days').value);
            var data = await request(root.dataset.issueUrl, body);
            var url = new URL(root.dataset.portalBase + get(data, 'token'), window.location.origin).href;
            document.getElementById('portal-link').value = url;
            document.getElementById('open-link').href = url;
            document.getElementById('issued-link').hidden = false;
            await refresh();
        } catch (error) { status.textContent = error.message; busy(false); }
    });
    revoke.addEventListener('click', async function () {
        if (!window.confirm('Thu hồi link tra cứu của khách hàng này?')) return;
        busy(true);
        try {
            await request(root.dataset.revokeUrl, form());
            document.getElementById('issued-link').hidden = true;
            document.getElementById('portal-link').value = '';
            document.getElementById('open-link').removeAttribute('href');
            await refresh();
        } catch (error) { status.textContent = error.message; busy(false); }
    });
    document.getElementById('copy-link').addEventListener('click', async function () {
        var field = document.getElementById('portal-link');
        try {
            await navigator.clipboard.writeText(field.value);
            status.textContent = 'Đã sao chép link khách hàng.';
        } catch (_) { field.focus(); field.select(); status.textContent = 'Nhấn Ctrl+C để sao chép link.'; }
    });
    refresh().catch(function (error) {
        document.getElementById('manage-customer').textContent = 'Không tải được thông tin khách hàng.';
        document.getElementById('history-status').textContent = 'Chưa tải được nhật ký. Vui lòng khắc phục lỗi truy cập rồi tải lại trang.';
        status.textContent = error.message; busy(false);
    });
}());
