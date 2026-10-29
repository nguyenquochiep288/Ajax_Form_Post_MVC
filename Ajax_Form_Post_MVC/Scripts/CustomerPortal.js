(function () {
    'use strict';
    var root = document.getElementById('customer-portal');
    if (!root) return;
    var content = document.getElementById('portal-content'), status = document.getElementById('portal-status');
    var from = document.getElementById('from-date'), to = document.getElementById('to-date'), search = document.getElementById('search');
    var dialog = document.getElementById('document-dialog'), documentContent = document.getElementById('document-content');
    var section = 'overview', page = 1, requestVersion = 0, detailVersion = 0, loading = false, valid = false;
    var customerName = '', chartData = null, lastChartWidth = 0;
    var names = { overview: 'Tổng quan giao dịch', orders: 'Đơn đặt hàng', outputs: 'Phiếu xuất', invoices: 'Hóa đơn', debt: 'Công nợ khách hàng', returns: 'Hàng trả lại', receipts: 'Phiếu thu', payments: 'Phiếu chi' };
    var number = new Intl.NumberFormat('vi-VN', { maximumFractionDigits: 2 });
    var currency = new Intl.NumberFormat('vi-VN', { maximumFractionDigits: 0 });
    function money(value) { return currency.format(value || 0); }
    function date(value) { return value ? String(value).slice(0, 10).split('-').reverse().join('/') : ''; }
    function normalize(value) {
        if (Array.isArray(value)) return value.map(normalize);
        if (value && typeof value === 'object') {
            var result = {};
            Object.keys(value).forEach(function (key) {
                var name = key === key.toUpperCase() ? key.toLowerCase() : key.charAt(0).toLowerCase() + key.slice(1);
                result[name] = normalize(value[key]);
            });
            return result;
        }
        return value;
    }
    function el(tag, text, className) {
        var node = document.createElement(tag);
        if (text !== null && text !== undefined) node.textContent = text;
        if (className) node.className = className;
        return node;
    }
    function svg(tag, attrs, text) {
        var node = document.createElementNS('http://www.w3.org/2000/svg', tag);
        Object.keys(attrs || {}).forEach(function (key) { node.setAttribute(key, attrs[key]); });
        if (text !== undefined) node.textContent = text;
        return node;
    }
    function panel(title, subtitle) {
        var node = el('section', null, 'panel');
        node.appendChild(el('h2', title));
        if (subtitle) node.appendChild(el('p', subtitle, 'panel-subtitle'));
        return node;
    }
    function setStatus(message, error) { status.textContent = message; status.classList.toggle('error', Boolean(error)); }
    function setBusy(value) { loading = value; content.setAttribute('aria-busy', String(value)); root.querySelector('#portal-filter button').disabled = value; }
    async function request(extra) {
        var body = new FormData();
        var params = Object.assign({ token: root.dataset.token, section: section, from: from.value, to: to.value, page: page, search: search.value }, extra || {});
        Object.keys(params).forEach(function (key) { if (params[key] !== undefined && params[key] !== null) body.append(key, params[key]); });
        var response = await fetch(root.dataset.readUrl, { method: 'POST', credentials: 'omit', cache: 'no-store', body: body });
        var data;
        try { data = normalize(await response.json()); } catch (_) { throw new Error('Chưa thể tải dữ liệu. Vui lòng thử lại.'); }
        if (!response.ok) { var error = new Error(data.message || 'Chưa thể tải dữ liệu.'); error.status = response.status; throw error; }
        return data;
    }
    async function load() {
        var version = ++requestVersion;
        valid = false;
        if (!from.value || !to.value || from.value > to.value || (new Date(to.value) - new Date(from.value)) / 86400000 > 1096) {
            content.replaceChildren(); chartData = null; setBusy(false);
            setStatus('Chọn khoảng thời gian hợp lệ, tối đa 3 năm.', true); return;
        }
        setBusy(true); setStatus('Đang tải dữ liệu…'); content.replaceChildren(); chartData = null;
        try {
            var data = await request();
            if (version !== requestVersion) return;
            if (section === 'overview') renderOverview(data);
            else if (section === 'debt') renderDebt(data);
            else renderList(data);
            valid = true;
            setStatus('Kỳ báo cáo: ' + date(from.value) + ' – ' + date(to.value));
            document.getElementById('updated-at').textContent = 'Cập nhật: ' + new Date().toLocaleString('vi-VN');
        } catch (error) {
            if (version !== requestVersion) return;
            setStatus(error.message, true);
            content.replaceChildren(el('p', 'Không có dữ liệu để hiển thị. Bạn có thể thử lại hoặc liên hệ Trang Hiệp Phát.', 'empty'));
            if (error.status === 404) { valid = false; customerName = ''; document.getElementById('customer-name').textContent = 'Trang khách hàng'; document.getElementById('customer-code').textContent = ''; }
        } finally { if (version === requestVersion) setBusy(false); }
    }
    function summaryCard(label, value, note) {
        var card = el('div', null, 'stat'); card.append(el('span', label, 'label'), el('strong', value), el('small', note)); return card;
    }
    function debtStats(summary) {
        var node = panel('Đối chiếu công nợ', 'Theo báo cáo công nợ khách hàng của hệ thống');
        var grid = el('div', null, 'debt-stats');
        [['Dư đầu kỳ', summary.opening], ['Xuất hàng trong kỳ', summary.outputs], ['Hàng trả lại', summary.returns], ['Đã thu trong kỳ', summary.receipts], ['Chi trả trong kỳ', summary.payments], ['Dư cuối kỳ', summary.closing]].forEach(function (pair) {
            var item = el('div'); item.append(el('span', pair[0]), el('strong', money(pair[1]) + ' đ')); grid.appendChild(item);
        });
        node.append(grid, el('p', 'Công nợ lấy từ phiếu xuất, hàng trả lại, phiếu thu và phiếu chi; không cộng thêm giá trị hóa đơn.', 'debt-note'));
        return node;
    }
    function renderOverview(data) {
        customerName = data.customer.name;
        document.getElementById('customer-name').textContent = customerName;
        document.getElementById('customer-code').textContent = data.customer.code;
        var stats = el('div', null, 'stats');
        function totals(kind) { return data.monthly.filter(function (x) { return x.kind === kind; }).reduce(function (acc, x) { acc.count += x.count; acc.total += x.total; return acc; }, { count: 0, total: 0 }); }
        var orders = totals('orders'), outputs = totals('outputs'), invoices = totals('invoices');
        stats.append(summaryCard('Đơn đặt hàng', number.format(orders.count), 'Giá trị: ' + money(orders.total) + ' đ'), summaryCard('Giá trị xuất hàng', money(outputs.total) + ' đ', outputs.count + ' phiếu xuất trong kỳ'), summaryCard('Giá trị hóa đơn', money(invoices.total) + ' đ', invoices.count + ' hóa đơn trong kỳ'), summaryCard('Công nợ cuối kỳ', money(data.debt.closing) + ' đ', 'Tại ngày ' + date(to.value)));
        content.appendChild(stats);
        var monthly = panel('Giao dịch theo tháng', 'So sánh giá trị đơn đặt hàng, phiếu xuất và hóa đơn · VNĐ');
        var legend = el('div', null, 'legend');
        ['Đặt hàng', 'Xuất hàng', 'Hóa đơn'].forEach(function (name, i) { var item = el('span'); item.append(el('i', null, 'c' + i), document.createTextNode(name)); legend.appendChild(item); });
        var chart = el('div', null, 'chart'); chart.id = 'monthly-chart'; chart.setAttribute('role', 'img'); chart.setAttribute('aria-label', 'Biểu đồ giao dịch theo tháng. Bảng số liệu ở bên dưới.');
        monthly.append(legend, chart); content.appendChild(monthly);
        chartData = data.monthly; drawMonthly();
        var details = el('details'); details.appendChild(el('summary', 'Xem bảng số liệu theo tháng'));
        details.appendChild(monthlyTable()); monthly.appendChild(details);
        var donuts = el('div', null, 'donut-grid');
        donuts.append(donutPanel('Trạng thái đơn hàng', data.states.filter(function (x) { return x.kind === 'orders'; }).map(function (x) { return { name: x.name, value: x.count }; }), 'đơn'),
            donutPanel('Trạng thái hóa đơn', data.states.filter(function (x) { return x.kind === 'invoices'; }).map(function (x) { return { name: x.name, value: x.count }; }), 'hóa đơn'),
            donutPanel('Nhóm sản phẩm đã xuất', data.groups.map(function (x) { return { name: x.name, value: x.total }; }), 'đ'));
        content.append(donuts, debtStats(data.debt));
        renderPayment();
        var recent = panel('Chứng từ gần nhất', '8 chứng từ gần nhất trong kỳ báo cáo'); recent.appendChild(documentTable(data.recent, true)); content.appendChild(recent);
    }
    function months() {
        var rows = [], start = new Date(from.value + 'T00:00:00'), end = new Date(to.value + 'T00:00:00');
        start.setDate(1);
        while (start <= end) {
            var row = { label: 'T' + (start.getMonth() + 1) + '/' + start.getFullYear(), orders: 0, outputs: 0, invoices: 0 };
            (chartData || []).filter(function (x) { return x.year === start.getFullYear() && x.month === start.getMonth() + 1; }).forEach(function (x) { row[x.kind] = x.total; });
            rows.push(row); start.setMonth(start.getMonth() + 1);
        }
        return rows;
    }
    function monthlyTable() {
        var table = makeTable(['Tháng', 'Đặt hàng (đ)', 'Xuất hàng (đ)', 'Hóa đơn (đ)']);
        months().forEach(function (x) { var row = el('tr'); [x.label, money(x.orders), money(x.outputs), money(x.invoices)].forEach(function (v, i) { row.appendChild(el('td', v, i ? 'num' : '')); }); table.body.appendChild(row); });
        return table.wrapper;
    }
    function drawMonthly() {
        var container = document.getElementById('monthly-chart');
        if (!container || !chartData) return;
        var width = Math.max(240, container.clientWidth), height = 300, left = 72, right = 10, top = 16, bottom = 48, rows = months();
        lastChartWidth = container.clientWidth;
        var values = rows.flatMap(function (x) { return [x.orders, x.outputs, x.invoices]; });
        var max = Math.max.apply(null, [1].concat(values)), min = Math.min.apply(null, [0].concat(values)), range = max - min;
        var h = height - top - bottom, w = width - left - right;
        var plot = svg('svg', { viewBox: '0 0 ' + width + ' ' + height, width: width, height: height });
        function y(value) { return top + (max - value) / range * h; }
        for (var i = 0; i <= 4; i++) {
            var value = min + range * i / 4, ypos = y(value);
            plot.append(svg('line', { x1: left, y1: ypos, x2: width - right, y2: ypos, 'class': 'grid' }), svg('text', { x: left - 9, y: ypos + 4, 'text-anchor': 'end' }, new Intl.NumberFormat('vi-VN', { notation: 'compact', maximumFractionDigits: 1 }).format(value)));
        }
        var step = w / rows.length, barWidth = Math.max(1, Math.min(18, step * .22)), labelStep = Math.max(1, Math.ceil(rows.length / Math.max(2, Math.floor(w / 78))));
        rows.forEach(function (row, index) {
            var center = left + step * (index + .5);
            ['orders', 'outputs', 'invoices'].forEach(function (kind, series) {
                var amount = row[kind], bar = svg('rect', { x: center + (series - 1.5) * barWidth, y: Math.min(y(amount), y(0)), width: Math.max(1, barWidth - 1), height: Math.abs(y(amount) - y(0)), 'class': 'series-' + kind });
                bar.appendChild(svg('title', {}, row.label + ' · ' + names[kind] + ': ' + money(amount) + ' đ')); plot.appendChild(bar);
            });
            if (index % labelStep === 0 || index === rows.length - 1 && rows.length % labelStep === 0) plot.appendChild(svg('text', { x: center, y: height - 20, 'text-anchor': 'middle' }, row.label));
        });
        container.replaceChildren(plot);
    }
    function donutPanel(title, values, unit) {
        var node = panel(title), layout = el('div', null, 'ring-layout');
        values.sort(function (a, b) { return b.value - a.value || a.name.localeCompare(b.name); });
        if (values.some(function (x) { return x.value < 0; })) {
            node.appendChild(el('p', 'Giá trị thuần theo nhóm (VNĐ)', 'panel-subtitle'));
            var signed = el('ul', null, 'ring-legend'); values.forEach(function (x) { var li = el('li'); li.append(el('span', x.name), el('strong', money(x.value))); signed.appendChild(li); });
            node.appendChild(signed); return node;
        }
        values = values.filter(function (x) { return x.value > 0; });
        if (values.length > 6) values = values.slice(0, 5).concat([{ name: 'Khác', value: values.slice(5).reduce(function (sum, x) { return sum + x.value; }, 0) }]);
        var total = values.reduce(function (sum, x) { return sum + x.value; }, 0);
        if (!total) { node.appendChild(el('p', 'Chưa có dữ liệu trong kỳ.', 'empty')); return node; }
        var ring = svg('svg', { viewBox: '0 0 132 132', 'class': 'ring', role: 'img', 'aria-label': title + ': ' + values.map(function (x) { return x.name + ' ' + money(x.value) + ' ' + unit; }).join(', ') });
        ring.appendChild(svg('circle', { cx: 66, cy: 66, r: 51, 'class': 'track' }));
        var circumference = 2 * Math.PI * 51, offset = 0, legend = el('ul', null, 'ring-legend');
        values.forEach(function (x, i) {
            var length = x.value / total * circumference;
            var arc = svg('circle', { cx: 66, cy: 66, r: 51, 'class': 'c' + i, 'stroke-dasharray': length + ' ' + (circumference - length), 'stroke-dashoffset': -offset, transform: 'rotate(-90 66 66)' });
            arc.appendChild(svg('title', {}, x.name + ': ' + money(x.value) + ' ' + unit)); ring.appendChild(arc); offset += length;
            var li = el('li'), text = el('span', x.name + ' · ' + money(x.value) + ' ' + unit);
            li.append(el('i', null, 'c' + i), text, el('strong', number.format(x.value / total * 100) + '%')); legend.appendChild(li);
        });
        ring.appendChild(svg('text', { x: 66, y: 72, 'text-anchor': 'middle' }, unit === 'đ' ? '100%' : number.format(total)));
        layout.append(ring, legend); node.appendChild(layout); return node;
    }
    function makeTable(headers) {
        var wrapper = el('div', null, 'table-scroll'), table = el('table'), head = el('thead'), body = el('tbody'), tr = el('tr');
        headers.forEach(function (name) { var th = el('th', name); th.scope = 'col'; tr.appendChild(th); });
        head.appendChild(tr); table.append(head, body); wrapper.appendChild(table); return { wrapper: wrapper, table: table, body: body };
    }
    function docButton(doc) {
        var button = el('button', doc.code || doc.invoiceNumber || '(Chưa có mã)', 'doc-link'); button.type = 'button';
        button.addEventListener('click', function () { showDocument(doc.kind, doc.id); }); return button;
    }
    function documentTable(rows, mixed) {
        if (!rows.length) return el('p', 'Không có chứng từ trong kỳ đã chọn.', 'empty');
        var table = makeTable(mixed ? ['Mã chứng từ', 'Loại chứng từ', 'Ngày', 'Giá trị (đ)', 'Trạng thái'] : ['Mã chứng từ', section === 'invoices' ? 'Ngày hóa đơn' : 'Ngày lập', 'Giá trị (đ)', 'Trạng thái'].concat(section === 'invoices' ? ['Số hóa đơn'] : []));
        rows.forEach(function (doc) {
            var row = el('tr'), code = el('td'); code.appendChild(docButton(doc)); row.appendChild(code);
            if (mixed) row.appendChild(el('td', names[doc.kind]));
            var state = el('td'); state.appendChild(el('span', doc.status, 'badge'));
            row.append(el('td', date(doc.date)), el('td', money(doc.total), 'num'), state);
            if (!mixed && section === 'invoices') row.appendChild(el('td', doc.invoiceNumber || '—'));
            table.body.appendChild(row);
        });
        return table.wrapper;
    }
    function pagination(data) {
        var node = el('div', null, 'pagination'), pages = Math.max(1, Math.ceil(data.total / data.pageSize));
        var prev = el('button', '← Trước', 'secondary'), next = el('button', 'Sau →', 'secondary'); prev.type = next.type = 'button';
        prev.disabled = data.page <= 1; next.disabled = data.page >= pages;
        prev.addEventListener('click', function () { if (!loading) { page--; load(); } });
        next.addEventListener('click', function () { if (!loading) { page++; load(); } });
        node.append(el('span', data.total + ' chứng từ · Trang ' + data.page + '/' + pages), prev, next); return node;
    }
    function renderList(data) {
        var node = panel(names[section], 'Tổng giá trị theo bộ lọc: ' + money(data.sum) + ' đ · Trang ' + data.page + ' · ' + data.rows.length + '/' + data.total + ' chứng từ'); node.append(documentTable(data.rows, false), pagination(data)); content.appendChild(node);
    }
    function renderDebt(data) {
        content.appendChild(debtStats(data.summary));
        var actions = el('div', null, 'export-actions');
        var excel = el('button', 'Xuất Excel'), pdf = el('button', 'Xuất PDF', 'secondary'); excel.type = pdf.type = 'button';
        excel.id = 'export-debt-excel'; pdf.id = 'export-debt-pdf';
        excel.addEventListener('click', function () { exportExcel(excel); });
        pdf.addEventListener('click', showStatement);
        actions.append(excel, pdf, el('span', 'Xuất toàn bộ phát sinh theo khoảng ngày đã chọn; không áp dụng tìm mã hoặc phân trang.', 'debt-note'));
        content.appendChild(actions);
        renderPayment();
        var node = panel('Phát sinh trong kỳ', 'Tăng nợ: xuất hàng, chi trả · Giảm nợ: hàng trả lại, thu tiền');
        if (!data.rows.length) node.appendChild(el('p', 'Không có phát sinh theo bộ lọc.', 'empty'));
        else {
            var table = makeTable(['Mã chứng từ', 'Loại', 'Ngày', 'Tăng nợ (đ)', 'Giảm nợ (đ)']);
            data.rows.forEach(function (entry) {
                var row = el('tr'), code = el('td'); code.appendChild(entry.kind === 'outputs' || entry.kind === 'receipts' ? docButton(entry) : document.createTextNode(entry.code || ''));
                row.append(code, el('td', names[entry.kind]), el('td', date(entry.date)), el('td', money(entry.increase), 'num'), el('td', money(entry.decrease), 'num')); table.body.appendChild(row);
            }); node.appendChild(table.wrapper);
        }
        node.appendChild(pagination(data)); content.appendChild(node);
        if (search.value.trim()) content.appendChild(el('p', 'Bộ lọc mã chỉ áp dụng cho danh sách phát sinh. Số dư công nợ luôn tính trên toàn bộ kỳ.', 'debt-note'));
    }
    function validExportDates() {
        if (!from.value || !to.value || from.value > to.value || (new Date(to.value) - new Date(from.value)) / 86400000 > 1096) {
            setStatus('Chọn khoảng thời gian hợp lệ, tối đa 3 năm.', true); return false;
        }
        return true;
    }
    async function exportExcel(button) {
        if (!validExportDates()) return;
        var exportFrom = from.value, exportTo = to.value;
        button.disabled = true; setStatus('Đang xuất toàn bộ bảng đối chiếu công nợ…');
        try {
            var body = new FormData(); body.append('token', root.dataset.token); body.append('from', exportFrom); body.append('to', exportTo);
            var response = await fetch(root.dataset.excelUrl, { method: 'POST', credentials: 'omit', cache: 'no-store', body: body });
            if (!response.ok) { var error = normalize(await response.json()); throw new Error(error.message || 'Không thể xuất Excel.'); }
            if (!(response.headers.get('content-type') || '').includes('spreadsheetml')) throw new Error('Phản hồi xuất Excel không hợp lệ.');
            var url = URL.createObjectURL(await response.blob()), link = el('a');
            link.href = url; link.download = 'doi-chieu-cong-no-' + exportFrom + '-' + exportTo + '.xlsx';
            document.body.appendChild(link); link.click(); link.remove(); setTimeout(function () { URL.revokeObjectURL(url); }, 1000);
            setStatus('Đã xuất Excel đối chiếu công nợ.');
        } catch (error) { setStatus(error.message, true); }
        finally { button.disabled = false; }
    }
    async function showStatement() {
        if (!validExportDates()) return;
        var version = ++detailVersion;
        document.getElementById('document-title').textContent = 'Bảng đối chiếu công nợ';
        documentContent.replaceChildren(el('p', 'Đang tải toàn bộ phát sinh trong kỳ…'));
        document.getElementById('print-document').disabled = true; dialog.showModal();
        try {
            var data = await request({ section: 'debt-export', id: '', search: '', page: 1 });
            if (version !== detailVersion || !dialog.open) return;
            var meta = el('div', null, 'statement-heading');
            meta.append(el('h3', 'TRANG HIỆP PHÁT'), el('p', 'Khách hàng: ' + data.customer.name + ' · Mã: ' + data.customer.code), el('p', 'Địa chỉ: ' + (data.customer.address || '')), el('p', 'Kỳ đối chiếu: ' + date(data.from) + ' – ' + date(data.to)), el('p', 'Đơn vị: VNĐ · ' + data.rows.length + ' phát sinh · Toàn bộ kỳ đối chiếu'));
            documentContent.replaceChildren(meta);

            var table = makeTable(['STT', 'Ngày', 'Mã chứng từ', 'Loại', 'Tăng nợ', 'Giảm nợ', 'Dư nợ']);
            var opening = el('tr'); opening.append(el('td', ''), el('td', ''), el('td', ''), el('td', 'Dư đầu kỳ'), el('td', '', 'num'), el('td', '', 'num'), el('td', money(data.summary.opening), 'num')); table.body.appendChild(opening);
            data.rows.forEach(function (item, index) {
                var row = el('tr'); [index + 1, date(item.date), item.code || '', names[item.kind], money(item.increase), money(item.decrease), money(item.balance)].forEach(function (value, i) { row.appendChild(el('td', value, i >= 4 ? 'num' : '')); }); table.body.appendChild(row);
            });
            var foot = el('tfoot'), total = el('tr');
            total.append(el('td', ''), el('td', ''), el('td', ''), el('th', 'Tổng / Dư tính toán'), el('td', money(data.increase), 'num'), el('td', money(data.decrease), 'num'), el('td', money(data.calculatedClosing), 'num')); foot.appendChild(total); table.table.appendChild(foot);
            documentContent.appendChild(table.wrapper);
            var closing = el('div', null, 'statement-closing');
            closing.append(el('p', 'Dư cuối kỳ theo hệ thống: ' + money(data.summary.closing) + ' đ', 'document-total'), el('p', 'Chênh lệch cần đối chiếu: ' + money(data.difference) + ' đ', Math.abs(data.difference) > .5 ? 'statement-warning' : 'debt-note'), el('p', 'Số dư âm thể hiện khách hàng trả trước / dư có. Hai bên xác nhận sau khi đối chiếu chứng từ.', 'debt-note'));
            var signatures = el('div', null, 'statement-signatures'); signatures.append(el('strong', 'ĐẠI DIỆN KHÁCH HÀNG'), el('strong', 'ĐẠI DIỆN TRANG HIỆP PHÁT')); closing.appendChild(signatures); documentContent.appendChild(closing);
            document.getElementById('print-document').disabled = false;
        } catch (error) { if (version === detailVersion) documentContent.replaceChildren(el('p', error.message)); }
    }
    function renderPayment() {
        var box = panel('Thông tin chuyển khoản', 'Tài khoản nhận tiền mặc định của Trang Hiệp Phát'), version = requestVersion;
        var details = el('div'), feedback = el('p', 'Đang tải thông tin chuyển khoản…', 'debt-note'); feedback.setAttribute('role', 'status');
        box.classList.add('payment-panel'); box.append(details, feedback); content.appendChild(box);
        function copyButton(label, value) {
            var button = el('button', label, 'secondary'); button.type = 'button';
            button.addEventListener('click', async function () {
                try { await navigator.clipboard.writeText(value); feedback.textContent = 'Đã sao chép.'; }
                catch (_) { var input = el('input'); input.value = value; input.readOnly = true; details.appendChild(input); input.focus(); input.select(); feedback.textContent = 'Nhấn Ctrl+C để sao chép thông tin.'; }
            }); return button;
        }
        request({ section: 'payment', amount: null, id: '' }).then(function (data) {
            if (version !== requestVersion || !box.isConnected) return;
            feedback.textContent = data.message || '';
            if (!data.available) return;
            var info = el('div', null, 'payment-info');
            info.append(el('p', 'Ngân hàng: ' + (data.bank.bankName || data.bank.bin)), el('p', 'Số tài khoản: ' + data.bank.accountNumber), el('p', 'Chủ tài khoản: ' + data.bank.accountName), el('p', 'Nội dung: ' + data.reference), copyButton('Sao chép số tài khoản', data.bank.accountNumber), copyButton('Sao chép nội dung', data.reference));
            var qr = el('div', null, 'payment-qr');
            function showQr(value) {
                qr.replaceChildren(); feedback.textContent = value.message || '';
                if (!value.qrDataUrl || !/^data:image\/png;base64,[A-Za-z0-9+/=]+$/.test(value.qrDataUrl)) return;
                var image = el('img'); image.src = value.qrDataUrl; image.alt = 'QR chuyển khoản đến ' + value.bank.accountName; image.width = image.height = 220;
                var download = el('a', 'Tải mã QR'); download.href = value.qrDataUrl; download.download = 'qr-chuyen-khoan.png';
                qr.append(image, el('p', value.amount ? 'Số tiền: ' + money(value.amount) + ' đ' : 'QR chưa đặt số tiền'), download);
            }
            showQr(data);
            var form = el('form', null, 'payment-form'), label = el('label', 'Số tiền muốn chuyển (VNĐ)'), amount = el('input');
            amount.type = 'number'; amount.min = '1'; amount.max = '9999999999999'; amount.step = '1'; amount.placeholder = 'Để trống để nhập trong ứng dụng ngân hàng'; amount.inputMode = 'numeric'; label.appendChild(amount);
            var generate = el('button', 'Tạo QR'); generate.type = 'submit'; form.append(label, generate);
            form.addEventListener('submit', async function (event) {
                event.preventDefault(); if (!form.reportValidity()) return;
                generate.disabled = true; qr.replaceChildren(); feedback.textContent = 'Đang tạo mã QR…';
                try {
                    var updated = await request({ section: 'payment', amount: amount.value || null, id: '' });
                    if (version !== requestVersion || !box.isConnected) return;
                    if (!updated.available) { details.replaceChildren(); feedback.textContent = updated.message; return; }
                    // Always refresh displayed recipient information together with its QR.
                    if (updated.bank.accountNumber !== data.bank.accountNumber || updated.bank.bin !== data.bank.bin || updated.bank.accountName !== data.bank.accountName) {
                        details.replaceChildren(); feedback.textContent = 'Thông tin nhận tiền vừa được thay đổi. Vui lòng tải lại báo cáo.'; return;
                    }
                    showQr(updated);
                } catch (error) { feedback.textContent = error.message; }
                finally { generate.disabled = false; }
            });
            var layout = el('div', null, 'payment-layout'); layout.append(info, qr); details.append(layout, form, el('p', 'Kiểm tra tên người nhận trong ứng dụng ngân hàng trước khi chuyển. Công nợ được cập nhật khi kế toán ghi nhận phiếu thu.', 'debt-note'));
        }).catch(function (error) { if (version === requestVersion && box.isConnected) feedback.textContent = error.message; });
    }
    async function showDocument(kind, id) {
        var version = ++detailVersion;
        document.getElementById('document-title').textContent = names[kind];
        documentContent.replaceChildren(el('p', 'Đang tải chi tiết…'));
        document.getElementById('print-document').disabled = true;
        dialog.showModal();
        try {
            var data = await request({ section: kind, id: id });
            if (version !== detailVersion || !dialog.open) return;
            var doc = data.document, meta = el('div', null, 'document-meta');
            meta.append(el('p', 'Khách hàng: ' + customerName), el('p', 'Mã chứng từ: ' + doc.code), el('p', 'Ngày: ' + date(doc.date)), el('p', 'Trạng thái: ' + doc.status));
            if (kind === 'invoices') {
                [['Số hóa đơn', doc.invoiceNumber], ['Ký hiệu', doc.invoiceSeries], ['Mã cơ quan thuế', doc.taxAuthorityCode]].forEach(function (pair) { meta.appendChild(el('p', pair[0] + ': ' + (pair[1] || '—'))); });
                var lookup = el('p', 'Mã tra cứu MISA: '), lookupCode = (doc.lookupCode || '').trim();
                if (lookupCode) {
                    var lookupLink = el('a', lookupCode);
                    lookupLink.href = 'https://meinvoice.vn/tra-cuu/?sc=' + encodeURIComponent(lookupCode);
                    lookupLink.target = '_blank';
                    lookupLink.rel = 'noopener noreferrer';
                    lookupLink.referrerPolicy = 'no-referrer';
                    lookup.appendChild(lookupLink);
                } else lookup.appendChild(document.createTextNode('—'));
                meta.appendChild(lookup);
            }
            documentContent.replaceChildren(meta);
            if (kind === 'receipts') {
                documentContent.append(el('p', 'Số tiền đã thu: ' + money(doc.total) + ' đ', 'document-total'), el('p', 'BẢN TRA CỨU · Trang Hiệp Phát', 'debt-note'));
                document.getElementById('print-document').disabled = false;
                return;
            }
            var table = makeTable(['Mã hàng', 'Tên hàng', 'ĐVT', 'Số lượng', 'Đơn giá', 'Giảm giá', 'Tiền thuế', 'Tổng cộng']);
            data.lines.forEach(function (line) {
                var row = el('tr'), nature = line.nature === 3 ? ' (Chiết khấu thương mại)' : '';
                [line.code || '', (line.name || '') + nature, line.unit || '', number.format(line.quantity), money(line.price), money(line.discount), money(line.tax), money(line.total)].forEach(function (value, i) { row.appendChild(el('td', value, i > 2 ? 'num' : '')); }); table.body.appendChild(row);
            });
            documentContent.appendChild(data.lines.length ? table.wrapper : el('p', 'Chứng từ chưa có dòng hàng.', 'empty'));
            var total = el('div', null, 'document-total'); total.append(el('p', 'Thành tiền: ' + money(doc.subtotal) + ' đ'), el('p', 'Giảm giá: ' + money(doc.discount) + ' đ'), el('p', 'Tiền thuế: ' + money(doc.tax) + ' đ'), el('strong', 'Tổng cộng: ' + money(doc.total) + ' đ'));
            documentContent.append(total, el('p', kind === 'invoices' ? 'BẢN TRA CỨU · Bản in này không thay thế hóa đơn điện tử gốc.' : 'BẢN TRA CỨU · Trang Hiệp Phát', 'debt-note'));
            document.getElementById('print-document').disabled = false;
        } catch (error) { if (version === detailVersion) documentContent.replaceChildren(el('p', error.message)); }
    }
    document.getElementById('close-document').addEventListener('click', function () { dialog.close(); });
    dialog.addEventListener('close', function () { detailVersion++; documentContent.replaceChildren(); root.classList.remove('print-document'); });
    document.getElementById('print-document').addEventListener('click', function () { root.classList.add('print-document'); window.print(); });
    document.getElementById('print-report').addEventListener('click', function () { if (valid && !loading) { root.classList.remove('print-document'); window.print(); } });
    window.addEventListener('afterprint', function () { root.classList.remove('print-document'); });
    document.getElementById('portal-filter').addEventListener('submit', function (event) { event.preventDefault(); page = 1; load(); });
    root.querySelectorAll('[data-section]').forEach(function (button) {
        button.addEventListener('click', function () {
            section = button.dataset.section; page = 1; search.value = '';
            root.querySelectorAll('[data-section]').forEach(function (item) { if (item === button) item.setAttribute('aria-current', 'page'); else item.removeAttribute('aria-current'); });
            document.getElementById('page-title').textContent = names[section]; document.getElementById('search-field').hidden = section === 'overview'; load();
        });
    });
    var today = new Date(Date.now() + 7 * 3600000).toISOString().slice(0, 10);
    var currentMonth = new Date(today + 'T00:00:00Z');
    from.value = today.slice(0, 7) + '-01';
    to.value = new Date(Date.UTC(currentMonth.getUTCFullYear(), currentMonth.getUTCMonth() + 1, 0)).toISOString().slice(0, 10);
    if (window.ResizeObserver) new ResizeObserver(function () { var chart = document.getElementById('monthly-chart'); if (chart && chart.clientWidth !== lastChartWidth) drawMonthly(); }).observe(content);
    else window.addEventListener('resize', drawMonthly);
    load();
}());
