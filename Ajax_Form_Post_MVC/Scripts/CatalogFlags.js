(function () {
    'use strict';
    function init() {
        var config = document.getElementById('catalog-flags-config');
        var body = document.getElementById('tbodytmpitem');
        if (!config || !body) return;
        var busy = false;
        var selector = 'td[id="ISACTIVE"] input[type="checkbox"], td[id="ISDEFAULT"] input[type="checkbox"]';
        var catalog = config.getAttribute('data-catalog');
        if (catalog === 'TypeInput') selector += ', td[id="ISNHAPTRAHANG"] input[type="checkbox"]';
        if (catalog === 'TypeOutput') selector += ', td[id="ISTRAHANG"] input[type="checkbox"]';
        var labels = { ISACTIVE: 'Hoạt động', ISDEFAULT: 'Mặc định', ISNHAPTRAHANG: 'Nhập hàng khách trả', ISTRAHANG: 'Xuất trả nhà cung cấp' };
        var message = document.createElement('div');
        message.className = 'catalog-flags-message';
        message.setAttribute('role', 'status');
        message.textContent = 'Tích/bỏ tích và xác nhận để lưu tại chỗ, không tải lại trang. Chọn mặc định mới sẽ bỏ mặc định cũ trong cùng danh mục.';
        var table = body.closest('.tablescroll') || body.closest('table');
        table.parentNode.insertBefore(message, table);

        function inputs() { return body.querySelectorAll(selector); }
        function prepare() {
            Array.prototype.forEach.call(inputs(), function (input) {
                input.disabled = busy;
                input.classList.add('catalog-flag');
                var label = labels[input.closest('td').id];
                var row = input.closest('tr');
                var name = row.querySelector('td[id="NAME"]') || row.querySelector('td[id="MA"]');
                input.setAttribute('aria-label', label + (name ? ': ' + name.textContent.trim() : ''));
                input.title = label === 'Mặc định' ? 'Chọn làm mặc định duy nhất; sẽ hỏi xác nhận trước khi lưu.' : 'Tích/bỏ tích; sẽ hỏi xác nhận trước khi lưu.';
            });
        }
        function show(text, error) {
            message.textContent = text;
            message.classList.toggle('text-danger', !!error);
            if (typeof window.gritter === 'function') {
                try { window.gritter(error ? 'Không lưu được' : 'Thông báo', text); } catch (ignored) { /* Status remains visible above the grid. */ }
            }
        }
        function clearOtherDefaults(id) {
            Array.prototype.forEach.call(body.querySelectorAll('td[id="ISDEFAULT"] input[type="checkbox"]'), function (input) {
                input.checked = input.closest('tr').id === id;
            });
        }
        window.CatalogFlags = {
            saved: function (response) {
                if (!response || !response.Success) return;
                var details = response.Detail || [];
                for (var i = 0; i < details.length; i++) {
                    if (details[i].Key === 'ISDEFAULT' && (details[i].Value === true || String(details[i].Value).toLowerCase() === 'true')) {
                        clearOtherDefaults(String(response.ID));
                        break;
                    }
                }
                prepare();
            }
        };
        // Prevent the existing drag-to-scroll grid from swallowing checkbox clicks.
        body.addEventListener('mousedown', function (event) {
            if (event.target.matches(selector)) event.stopPropagation();
        });
        body.addEventListener('change', function (event) {
            var input = event.target;
            if (!input.matches(selector)) return;
            var enabled = input.checked;
            if (busy) { input.checked = !enabled; return; }
            var id = input.closest('tr').id;
            var field = input.closest('td').id;
            var row = input.closest('tr');
            var codeCell = row.querySelector('td[id="MA"]');
            var nameCell = row.querySelector('td[id="NAME"]');
            var rowName = [codeCell ? codeCell.textContent.trim() : '', nameCell ? nameCell.textContent.trim() : ''].filter(Boolean).join(' - ');
            var prompt = 'Bạn có muốn ' + (enabled ? 'bật' : 'bỏ') + ' "' + labels[field] + '" cho ' + (rowName || 'dòng này') + '?';
            if (field === 'ISDEFAULT' && enabled) prompt += '\nMặc định cũ trong cùng danh mục và chi nhánh sẽ được bỏ chọn, kể cả ở trang khác.';
            if (!window.confirm(prompt)) {
                input.checked = !enabled;
                return;
            }
            var isReturnFlag = field === 'ISNHAPTRAHANG' || field === 'ISTRAHANG';
            var returnForm = isReturnFlag ? row.querySelector('form.return-flag-form') : null;
            if (isReturnFlag && !returnForm) {
                input.checked = !enabled;
                show('Không tìm thấy thông tin lưu phân loại trả hàng.', true);
                return;
            }
            var token = config.querySelector('input[name="__RequestVerificationToken"]');
            busy = true;
            prepare();
            message.textContent = 'Đang lưu thay đổi...';
            message.classList.remove('text-danger');
            var request = new XMLHttpRequest();
            request.open('POST', isReturnFlag ? returnForm.action : config.getAttribute('data-url'), true);
            request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
            request.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
            request.timeout = 45000;
            function fail(text) { input.checked = !enabled; show(text, true); }
            request.onload = function () {
                try {
                    var data = JSON.parse(request.responseText);
                    if (request.status >= 200 && request.status < 300 && data.Success) {
                        if (field === 'ISDEFAULT' && enabled) clearOtherDefaults(id);
                        show('Đã lưu ' + labels[field] + '.', false);
                    } else {
                        fail(data.Message || 'Không lưu được. Vui lòng tải lại danh sách và thử lại.');
                    }
                } catch (error) {
                    fail(request.status === 401 || request.status === 403
                        ? 'Phiên đăng nhập hết hạn hoặc bạn không có quyền sửa danh mục.'
                        : 'Không nhận được kết quả lưu. Vui lòng tải lại danh sách để kiểm tra.');
                }
            };
            request.onerror = request.ontimeout = function () {
                fail('Mất kết nối hoặc hết thời gian chờ. Vui lòng tải lại danh sách để kiểm tra trạng thái đã lưu.');
            };
            request.onloadend = function () { busy = false; prepare(); };
            request.send('catalog=' + encodeURIComponent(config.getAttribute('data-catalog'))
                + '&id=' + encodeURIComponent(id) + '&field=' + encodeURIComponent(field)
                + '&enabled=' + enabled + '&__RequestVerificationToken=' + encodeURIComponent(token.value));
        });
        // New rows are cloned by Category.js after creating a catalog item.
        new MutationObserver(prepare).observe(body, { childList: true, subtree: true });
        prepare();
    }
    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init);
    else init();
})();
