(function () {
    'use strict';
    function init() {
        var root = document.getElementById('inventoryInsights');
        if (!root) return;
        var mode = root.getAttribute('data-mode'), page = 1, pages = 1, busy = false, request = null, version = 0, snapshot = null, loadingTimer = null;
        var titles = { slow: 'Hàng tồn lâu, chậm luân chuyển', reorder: 'Hàng sắp hết và đề xuất nhập', sellers: 'Hàng bán chạy / ít bán' };
        var definitions = {
            slow: 'Lọc hàng còn tồn và không bán từ ngưỡng ngày đã chọn, hoặc số ngày đủ bán đạt ngưỡng. Hàng vẫn bán có thể xuất hiện nếu tồn quá nhiều so với bán ròng bình quân trong kỳ; xem cột Lý do. Ngày không bán tính đến hôm nay, nếu chưa từng bán thì tính từ lần nhập đầu không phải chuyển kho. Thiếu lịch sử được thông báo riêng, không tự xếp là tồn lâu. Đây không phải tuổi tồn theo lô/FIFO.',
            reorder: 'Điểm đặt hàng = bán ròng bình quân/ngày × thời gian chờ nhập + tồn an toàn. Đề xuất = làm tròn lên MAX(0; bình quân/ngày × (chờ nhập + dự trữ thêm) + tồn an toàn − tồn hiện tại). Chưa trừ hàng đang đặt mua; cần xem lại trước khi nhập. Không có sức bán và tồn an toàn bằng 0 thì chưa có cơ sở đề xuất.',
            sellers: 'Xếp hạng từng sản phẩm/kho theo bán ròng quy đổi về đơn vị gốc: (xuất bán − nhập khách trả) / tỷ lệ quy đổi. Bán nhiều xếp giảm dần, bán ít xếp tăng dần; tính cả phần lẻ. Cùng số lượng thì xét doanh thu, rồi mã hàng. Thứ tự tính trên toàn bộ kết quả trước khi chia trang. Thùng/bao/cây có quy cách khác nhau; xem thêm tỷ lệ quy đổi khi so sánh.'
        };
        function el(id) { return document.getElementById(id); }
        function num(v) { return v == null ? '—' : Number(v).toLocaleString('vi-VN', { maximumFractionDigits: 2 }); }
        function packed(v, row) {
            if (v == null || v === '' || !isFinite(Number(v))) return '—';
            var quantity = Number(v), ratio = Number(row.RATIO);
            if (!isFinite(ratio) || ratio <= 1) return num(quantity) + ' ' + row.UNIT;
            // Round only the displayed quantity, then split it into full packs and loose units.
            var absolute = Math.round(Math.abs(quantity) * 100) / 100;
            var packs = Math.floor(absolute / ratio);
            var remainder = Math.round((absolute - packs * ratio) * 100) / 100;
            if (remainder >= ratio) { packs++; remainder = 0; }
            var parts = [];
            if (packs > 0 || remainder === 0) parts.push(num(packs) + ' ' + row.BASE_UNIT);
            if (remainder > 0) parts.push(num(remainder) + ' ' + row.UNIT);
            var text = parts.join(' ');
            return quantity < 0 && absolute > 0 ? (parts.length > 1 ? '-(' + text + ')' : '-' + text) : text;
        }
        function baseQuantity(v, row) { return v == null ? '—' : num(v) + ' ' + row.BASE_UNIT; }
        function date(v) { if (!v) return '—'; var d = new Date(v); return isNaN(d.getTime()) ? '—' : d.toLocaleDateString('vi-VN'); }
        function field(obj, key) { var found = Object.keys(obj || {}).filter(function (k) { return k.toUpperCase() === key.toUpperCase(); })[0]; return found == null ? null : obj[found]; }
        function normalize(row) { var result = {}; Object.keys(row).forEach(function (k) { result[k.toUpperCase()] = row[k]; }); return result; }
        // Keep dragging local to this report; Footer.js handles other .tablescroll grids.
        var grid = root.querySelector('.ii-table'), drag = null, suppressClick = false;
        function stopGridDrag() {
            if (!drag) return;
            var pointerId = drag.id;
            drag = null;
            grid.classList.remove('ii-dragging');
            if (grid.hasPointerCapture(pointerId)) grid.releasePointerCapture(pointerId);
        }
        grid.addEventListener('pointerdown', function (event) {
            suppressClick = false;
            // Touch keeps native scrolling. Ctrl/Command allows selecting text to copy.
            if (event.pointerType !== 'mouse' || event.button !== 0 || event.ctrlKey || event.metaKey || event.shiftKey) return;
            if (!event.target.closest('td,th') || event.target.closest('a,button,input,select,textarea,[contenteditable="true"]')) return;
            if (grid.scrollWidth <= grid.clientWidth) return;
            drag = { id: event.pointerId, x: event.clientX, left: grid.scrollLeft, moved: false };
            grid.setPointerCapture(event.pointerId);
            event.preventDefault();
        });
        grid.addEventListener('pointermove', function (event) {
            if (!drag || drag.id !== event.pointerId) return;
            if ((event.buttons & 1) === 0) { stopGridDrag(); return; }
            var distance = event.clientX - drag.x;
            if (!drag.moved && Math.abs(distance) < 4) return;
            drag.moved = true;
            suppressClick = true;
            grid.classList.add('ii-dragging');
            grid.scrollLeft = drag.left - distance;
            event.preventDefault();
        });
        grid.addEventListener('pointerup', stopGridDrag);
        grid.addEventListener('pointercancel', stopGridDrag);
        grid.addEventListener('lostpointercapture', stopGridDrag);
        window.addEventListener('blur', stopGridDrag);
        grid.addEventListener('click', function (event) {
            if (suppressClick) { event.preventDefault(); event.stopPropagation(); suppressClick = false; }
        }, true);
        function columns() {
            var common = [['CODE', 'Mã hàng'], ['NAME', 'Tên sản phẩm'], ['GROUP', 'Nhóm'], ['DEPOT', 'Kho'], ['UNIT', 'ĐVT tồn kho'],
                ['CONVERSION', 'Quy đổi'], ['STOCK', 'Tồn theo quy cách', packed], ['SOLD', 'Xuất bán trong kỳ', packed], ['RETURNED', 'Nhập khách trả (có giá)', packed], ['SUPPLIER_RETURNED', 'Xuất trả NCC', packed], ['NET_SOLD', 'Bán ròng', packed]];
            if (mode === 'slow') return common.concat([['LAST_RECEIPT', 'Nhập gần nhất', date], ['LAST_SALE', 'Bán gần nhất', date], ['IDLE_DAYS', 'Ngày không bán đến hôm nay', num], ['COVER_DAYS', 'Ngày đủ bán', num], ['NOTE', 'Lý do']]);
            if (mode === 'reorder') return common.concat([['DAILY_SALES', 'Bán bình quân/ngày', packed], ['COVER_DAYS', 'Ngày đủ bán', num], ['REORDER_POINT', 'Điểm đặt hàng', packed], ['SUGGESTED', 'Đề xuất theo quy cách', packed], ['SUGGESTED_BASE', 'Đề xuất tròn ĐVT gốc', baseQuantity], ['STATUS', 'Trạng thái']]);
            return [['RANK_POSITION', 'Thứ tự', num]].concat(common, [['REVENUE', 'Doanh thu ròng chưa VAT', num], ['DAILY_SALES', 'Bán bình quân/ngày', packed], ['LAST_SALE', 'Bán gần nhất', date]]);
        }
        function prepare(row) {
            row.CONVERSION = '1 ' + row.BASE_UNIT + ' / ' + num(row.RATIO) + ' ' + row.UNIT;
            row.NOTE = row.NOTE || 'Chưa có lý do đánh giá; cần cập nhật API.';
            return row;
        }
        function render(rows, emptyMessage) {
            stopGridDrag();
            var cols = columns(), head = el('ii-head'), body = el('ii-body');
            head.innerHTML = ''; body.innerHTML = '';
            var tr = document.createElement('tr');
            cols.forEach(function (col) { var th = document.createElement('th'); th.textContent = col[1]; tr.appendChild(th); }); head.appendChild(tr);
            rows.forEach(function (row) {
                prepare(row); var line = document.createElement('tr');
                line.className = mode === 'reorder' && row.STOCK <= 0 ? 'ii-alert' : mode === 'slow' ? 'ii-warning' : '';
                cols.forEach(function (col) {
                    var td = document.createElement('td'); td.setAttribute('data-column', col[0]); td.textContent = col[2] ? col[2](row[col[0]], row) : row[col[0]] == null ? '—' : row[col[0]];
                    if (col[2] === num) td.className = 'ii-number';
                    if (col[2] === packed || col[2] === baseQuantity) td.className = 'ii-quantity';
                    line.appendChild(td);
                }); body.appendChild(line);
            });
            if (!rows.length) { var empty = document.createElement('tr'), cell = document.createElement('td'); cell.colSpan = cols.length; cell.textContent = emptyMessage || 'Không có dữ liệu phù hợp với điều kiện đã chọn.'; empty.appendChild(cell); body.appendChild(empty); }
        }
        function error(text) { el('ii-error').textContent = text || ''; el('ii-error').hidden = !text; }
        function stopLoadingTimer() { if (loadingTimer != null) { clearInterval(loadingTimer); loadingTimer = null; } }
        function controls() { el('ii-prev').disabled = busy || !snapshot || page <= 1; el('ii-next').disabled = busy || !snapshot || page >= pages; el('ii-run').disabled = busy; el('ii-export').disabled = busy || !snapshot; el('ii-cancel').hidden = !busy; root.setAttribute('aria-busy', String(busy)); }
        function invalidate() {
            version++; if (request) { request.abort(); request = null; }
            stopLoadingTimer(); busy = false; snapshot = null; page = pages = 1; render([], 'Chưa tải dữ liệu. Chọn điều kiện rồi bấm Lọc báo cáo.'); error('');
            el('ii-summary').textContent = 'Chọn điều kiện rồi bấm Lọc báo cáo.'; el('ii-page').textContent = ''; controls();
        }
        function groupSummary() {
            var selected = Array.from(root.querySelectorAll('.ii-group-list input:checked'));
            el('ii-group-summary').textContent = selected.length ? 'Đã chọn ' + selected.length + ' nhóm: ' + selected.map(function (c) { return c.parentNode.textContent.trim(); }).join(', ') : 'Tất cả nhóm hàng hóa';
        }
        function switchMode(next) {
            mode = next; el('ii-title').textContent = titles[mode]; el('ii-definition').textContent = definitions[mode];
            root.querySelectorAll('[data-mode]').forEach(function (b) { b.classList.toggle('is-active', b.getAttribute('data-mode') === mode); b.setAttribute('aria-pressed', String(b.getAttribute('data-mode') === mode)); });
            root.querySelectorAll('[data-for]').forEach(function (section) { var visible = section.getAttribute('data-for') === mode; section.hidden = !visible; section.querySelectorAll('input,select').forEach(function (input) { input.disabled = !visible; }); });
            invalidate();
        }
        function filters() {
            return { MODE: mode, FROM: el('ii-from').value, TO: el('ii-to').value, DEPOT: el('ii-depot').value, SEARCH: el('ii-search').value.trim(),
                GROUPS: Array.from(root.querySelectorAll('.ii-group-list input:checked')).map(function (c) { return c.value; }),
                IDLE_DAYS: Number(el('ii-idle').value), LEAD_DAYS: Number(el('ii-lead').value), COVER_DAYS: Number(el('ii-cover').value), MIN_STOCK: Number(el('ii-min').value),
                RANK: el('ii-rank').value, INCLUDE_INACTIVE: el('ii-inactive').checked };
        }
        function exportCsv(rows) {
            var cols = columns();
            function quote(value) { var s = String(value == null ? '' : value); if (/^[=+@\-\t\r]/.test(s)) s = "'" + s; return '"' + s.replace(/"/g, '""') + '"'; }
            var lines = [cols.map(function (c) { return quote(c[1]); }).join(';')];
            rows.forEach(function (row) { prepare(row); lines.push(cols.map(function (c) { return quote(c[2] ? c[2](row[c[0]], row) : row[c[0]]); }).join(';')); });
            var url = URL.createObjectURL(new Blob(['\uFEFF' + lines.join('\r\n')], { type: 'text/csv;charset=utf-8;' }));
            var a = document.createElement('a'); a.href = url; a.download = 'BaoCao_' + mode + '_' + snapshot.FROM + '_' + snapshot.TO + '.csv'; a.click(); setTimeout(function () { URL.revokeObjectURL(url); }, 1000);
        }
        function load(targetPage, exporting) {
            if (busy) return;
            var filter = snapshot || filters(), payload = Object.assign({}, filter, { PAGE: targetPage, EXPORT: !!exporting }), current = ++version;
            busy = true; error(''); controls();
            el('ii-summary').textContent = exporting ? 'Đang xuất toàn bộ kết quả theo bộ lọc…' : 'Đang tải báo cáo…';
            if (!exporting) render([], 'Đang lấy dữ liệu từ máy chủ, chưa có kết quả báo cáo.');
            var started = Date.now();
            stopLoadingTimer();
            loadingTimer = setInterval(function () {
                if (current === version && busy) el('ii-summary').textContent = (exporting ? 'Đang xuất báo cáo… ' : 'Đang tải báo cáo… ') + Math.floor((Date.now() - started) / 1000) + ' giây';
            }, 1000);
            request = window.jQuery.ajax({ url: root.getAttribute('data-url'), type: 'POST', dataType: 'json', timeout: 125000,
                data: { __RequestVerificationToken: root.querySelector('input[name="__RequestVerificationToken"]').value, payload: JSON.stringify(payload) },
                success: function (response) {
                    if (current !== version) return;
                    try {
                    if (!field(response, 'Success')) { fail(field(response, 'Message') || 'Không tải được báo cáo.'); return; }
                    var data = field(response, 'Data'); if (typeof data === 'string') { try { data = JSON.parse(data); } catch (_) { fail('Dữ liệu báo cáo không hợp lệ.'); return; } }
                    var values = field(data, 'ROWS'); if (!Array.isArray(values)) { fail('API chưa trả đúng danh sách báo cáo. Vui lòng cập nhật API và MVC cùng phiên bản.'); return; }
                    var rows = values.map(normalize); snapshot = filter;
                    if (exporting) exportCsv(rows); else { render(rows); page = Number(field(data, 'PAGE')); pages = Number(field(data, 'PAGES')); }
                    el('ii-summary').textContent = num(field(data, 'TOTAL')) + ' dòng phù hợp · Tồn kho cập nhật: ' + new Date(field(data, 'SNAPSHOT')).toLocaleString('vi-VN');
                    if (mode === 'slow' && Number(field(data, 'MISSING_HISTORY_COUNT')) > 0)
                        el('ii-summary').textContent += ' · ' + num(field(data, 'MISSING_HISTORY_COUNT')) + ' dòng còn tồn thiếu lịch sử, chưa đủ cơ sở đánh giá tồn lâu';
                    el('ii-page').textContent = 'Trang ' + page + ' / ' + pages;
                    } catch (_) { fail('Không đọc được kết quả báo cáo. Vui lòng cập nhật API và MVC cùng phiên bản rồi tải lại.'); }
                },
                error: function (xhr, status) { if (current === version && status !== 'abort') fail(status === 'timeout' ? 'Máy chủ chưa trả kết quả trong thời gian chờ. Hãy chọn kho/nhóm hàng và thử lại.' : 'Không tải được báo cáo. Kiểm tra kết nối hoặc chọn lại phạm vi.'); },
                complete: function () { if (current === version) { stopLoadingTimer(); request = null; busy = false; controls(); } }
            });
            function fail(text) { if (!exporting) { snapshot = null; render([], 'Tải báo cáo không thành công. Xem thông báo lỗi phía trên.'); el('ii-page').textContent = ''; } el('ii-summary').textContent = 'Chưa tải được kết quả.'; error(text); }
        }
        el('ii-filter').onsubmit = function (e) { e.preventDefault(); invalidate(); load(1, false); };
        el('ii-cancel').onclick = function () { invalidate(); el('ii-summary').textContent = 'Đã dừng chờ báo cáo. Bạn có thể chọn lại điều kiện và tải lại.'; };
        el('ii-filter').addEventListener('change', function () { groupSummary(); invalidate(); });
        el('ii-search').addEventListener('input', invalidate);
        root.querySelectorAll('.ii-tabs button').forEach(function (button) { button.onclick = function () { switchMode(button.getAttribute('data-mode')); }; });
        el('ii-group-search').oninput = function () { var term = this.value.toLocaleLowerCase('vi'); root.querySelectorAll('.ii-group-list label').forEach(function (label) { label.hidden = label.textContent.toLocaleLowerCase('vi').indexOf(term) < 0; }); };
        el('ii-groups-all').onclick = function () { root.querySelectorAll('.ii-group-list label:not([hidden]) input').forEach(function (c) { c.checked = true; }); groupSummary(); invalidate(); };
        el('ii-groups-clear').onclick = function () { root.querySelectorAll('.ii-group-list input').forEach(function (c) { c.checked = false; }); groupSummary(); invalidate(); };
        el('ii-prev').onclick = function () { if (page > 1) load(page - 1, false); }; el('ii-next').onclick = function () { if (page < pages) load(page + 1, false); };
        el('ii-export').onclick = function () { if (snapshot) load(page, true); };
        switchMode(mode);
    }
    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init); else init();
}());
