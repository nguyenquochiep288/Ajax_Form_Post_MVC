(function () {
    'use strict';
    var products = [], page = 0, pageSize = 36, request = null, customerRequest = null;
    var root, form, busy = false, saved = false, customerBusy = false;
    var selectedWarehouse = '', selectedCustomer = '', pendingProduct = null, productBusy = false;
    var groupTimer = null;
    function el(id) { return document.getElementById(id); }
    function hasCartItems() { return !!el('tbodyTempItemInput').querySelector('input[name^="txt"]'); }
    function retainCartScope(id, previous) {
        if (hasCartItems() && el(id).value !== previous) {
            el(id).value = previous;
            window.jQuery(el(id)).trigger('chosen:updated');
            message('Đơn hàng đã có sản phẩm. Xóa các hàng đã chọn trước khi đổi kho hoặc khách hàng.', true);
            return false;
        }
        return true;
    }
    function message(text, error) {
        el('om-save-status').textContent = text || '';
        el('om-save-status').className = error ? 'text-danger' : 'text-success';
        el('om-selection-status').textContent = text || '';
        el('om-selection-status').className = error ? 'alert alert-danger' : 'alert alert-info';
        el('om-selection-status').hidden = !text;
    }
    function selectProduct(product, button) {
        if (saved || busy || productBusy) return;
        pendingProduct = { product: product, button: button };
        if (!el('lstdm_KhachHang').value) {
            message('Chọn khách hàng ở phần Thông tin đơn hàng; sau đó sản phẩm sẽ mở để nhập số lượng.', true);
            el('collapseOneDeposit').scrollIntoView({ behavior: 'smooth', block: 'center' });
            window.jQuery('#lstdm_KhachHang').trigger('chosen:open');
            return;
        }
        if (customerBusy) { message('Đang tải khách hàng. Sản phẩm sẽ mở khi tải xong.'); return; }
        pendingProduct = null;
        if (!el('myModalDeposit_Temp') || typeof window.OnSuccessDeposit_Temp !== 'function') {
            message('Không tải được cửa sổ chọn hàng. Vui lòng tải lại trang.', true);
            return;
        }
        productBusy = true;
        button.disabled = true;
        message('Đang mở ' + product.Name + '…');
        window.jQuery.ajax({
            url: root.getAttribute('data-product-url'), type: 'POST', dataType: 'json',
            data: { id: product.Id }, timeout: 30000,
            success: function (data) {
                if (data.URL) { window.location.href = data.URL; return; }
                if (!data.Success || !Array.isArray(data.Detail)) {
                    message(data.Message || 'Không tải được thông tin sản phẩm. Vui lòng chọn lại.', true);
                    return;
                }
                if (window.OnSuccessDeposit_Temp(data) === false) {
                    message('Không mở được cửa sổ chọn hàng. Vui lòng tải lại trang rồi thử lại.', true);
                    return;
                }
                root.querySelectorAll('.activeDeposit').forEach(function (item) { item.classList.remove('activeDeposit'); });
                button.classList.add('activeDeposit');
                message('');
            },
            error: function (xhr, status) {
                message(status === 'parsererror'
                    ? 'Phiên đăng nhập hoặc dữ liệu sản phẩm không hợp lệ. Vui lòng tải lại trang và đăng nhập lại nếu được yêu cầu.'
                    : 'Không tải được sản phẩm. Vui lòng bấm Chọn hàng để thử lại.', true);
            },
            complete: function () { productBusy = false; button.disabled = false; }
        });
    }
    function textNode(tag, className, text) {
        var node = document.createElement(tag);
        node.className = className;
        node.textContent = text || '';
        return node;
    }
    function number(value) { return Number(value).toLocaleString('vi-VN', { maximumFractionDigits: 2 }); }
    function render() {
        var container = el('om-products');
        container.innerHTML = '';
        products.slice(page * pageSize, (page + 1) * pageSize).forEach(function (product) {
            var button = document.createElement('button');
            button.type = 'button';
            button.className = 'om-product productDeposit-button';
            button.setAttribute('aria-label', 'Chọn ' + product.Name);
            var img = document.createElement('img');
            img.src = product.Image;
            img.alt = product.Name;
            img.loading = 'lazy';
            img.onerror = function () { this.onerror = null; this.src = '/Images_Upload/Product/NoImage.png'; };
            button.appendChild(img);
            var info = textNode('span', 'om-product-info', '');
            info.appendChild(textNode('span', 'om-product-code', product.Code));
            info.appendChild(textNode('span', 'om-product-name', product.Name));
            info.appendChild(textNode('span', 'om-product-price', number(product.Price) + ' đ / ' + (product.Unit || '')));
            if (product.Stock != null) info.appendChild(textNode('span', 'om-product-stock', 'Tồn: ' + number(product.Stock) + ' ' + (product.StockUnit || '')));
            button.appendChild(info);
            button.appendChild(textNode('span', 'om-product-action', '+ Chọn hàng'));
            button.onclick = function () {
                selectProduct(product, button);
            };
            container.appendChild(button);
        });
        var pages = Math.max(1, Math.ceil(products.length / pageSize));
        el('om-status').textContent = products.length ? products.length + ' sản phẩm phù hợp' : 'Không tìm thấy sản phẩm. Hãy đổi nhóm, từ khóa hoặc chọn hiện cả hàng hết tồn kho.';
        el('om-page-info').textContent = 'Trang ' + (page + 1) + ' / ' + pages;
        el('om-prev').disabled = page === 0;
        el('om-next').disabled = page + 1 >= pages;
    }
    function loadProducts() {
        if (groupTimer) { clearTimeout(groupTimer); groupTimer = null; }
        if (request) request.abort();
        products = []; page = 0; render();
        if (!el('lstdm_Kho').value) { el('om-status').textContent = 'Vui lòng chọn kho để tải sản phẩm.'; return; }
        el('om-status').textContent = 'Đang tải sản phẩm…';
        el('om-products').setAttribute('aria-busy', 'true');
        request = window.jQuery.ajax({
            url: root.getAttribute('data-products-url'), type: 'POST', dataType: 'json',
            traditional: true,
            data: { ID_KHO: el('lstdm_Kho').value, GroupIDs: selectedGroupIds(), keySearch: el('om-search').value.trim(), BOLTONKHO: el('om-include-empty').checked },
            success: function (data) {
                if (data.URL) { window.location.href = data.URL; return; }
                if (!data.Success) { el('om-status').textContent = data.Message || 'Không tải được sản phẩm.'; return; }
                products = data.Products || []; render();
            },
            error: function (xhr, status) { if (status !== 'abort') el('om-status').textContent = 'Không tải được sản phẩm. Bấm Tìm kiếm để thử lại.'; },
            complete: function () { el('om-products').setAttribute('aria-busy', 'false'); }
        });
    }
    function selectedGroupIds() {
        return Array.from(el('om-group-options').querySelectorAll('input:checked')).map(function (input) { return input.value; });
    }
    function groupsChanged() {
        var checked = Array.from(el('om-group-options').querySelectorAll('input:checked'));
        el('om-group-summary').textContent = checked.length ? 'Đã chọn ' + checked.length + ' nhóm' : 'Tất cả nhóm hàng';
        el('om-group-summary').title = checked.map(function (input) { return input.parentNode.textContent.trim(); }).join(', ');
        if (groupTimer) clearTimeout(groupTimer);
        groupTimer = setTimeout(loadProducts, 250);
    }
    function groupSearchText(value) {
        return value.toLocaleLowerCase('vi').normalize('NFD').replace(/[\u0300-\u036f]/g, '').replace(/đ/g, 'd').trim();
    }
    function initGroups() {
        var menu = el('om-group-menu'), options = el('om-group-options');
        options.addEventListener('change', groupsChanged);
        el('om-group-search').oninput = function () {
            var term = groupSearchText(this.value), visible = 0;
            options.querySelectorAll('label').forEach(function (label) {
                label.hidden = groupSearchText(label.textContent).indexOf(term) < 0;
                if (!label.hidden) visible++;
            });
            el('om-group-empty').hidden = visible > 0;
        };
        el('om-group-search').onkeydown = function (event) {
            if (event.key === 'Enter') event.preventDefault();
        };
        el('om-groups-all').onclick = function () {
            options.querySelectorAll('label:not([hidden]) input').forEach(function (input) { input.checked = true; });
            groupsChanged();
        };
        el('om-groups-clear').onclick = function () {
            options.querySelectorAll('input').forEach(function (input) { input.checked = false; });
            groupsChanged();
        };
        menu.addEventListener('toggle', function () { if (menu.open) el('om-group-search').focus(); });
        menu.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') { menu.open = false; menu.querySelector('summary').focus(); }
        });
        document.addEventListener('click', function (event) { if (!menu.contains(event.target)) menu.open = false; });
    }
    function setBusy(value) {
        busy = value;
        form.querySelectorAll('button[type="submit"]').forEach(function (button) { button.disabled = value || saved; });
    }
    window.orderMenuBegin = function () {
        if (busy || saved || customerBusy || productBusy) return false;
        if (!el('tbodyTempItemInput').querySelector('input[name^="txt"]')) {
            message('Vui lòng thêm ít nhất một sản phẩm vào đơn hàng.', true);
            return false;
        }
        setBusy(true);
        message('Đang xử lý đơn hàng…');
        return true;
    };
    window.orderMenuSaved = function (data) {
        setBusy(false);
        if (data.URL) { window.location.href = data.URL; return; }
        if (!data.Success) {
            message(data.Message || 'Chưa lưu được đơn hàng. Vui lòng kiểm tra thông tin.', true);
            if (Array.isArray(data.Data)) {
                var errors = data.Data.map(function (entry) { return entry.Error; }).filter(Boolean);
                if (errors.length) message(errors.join(' · '), true);
            }
            return;
        }
        if (data.TYPE === 'Save') {
            saved = true;
            setBusy(false);
            message('Đã lưu đơn hàng thành công. Về danh sách phiếu đặt hàng để xem đơn vừa tạo.');
            // The server clears its cart after saving; prevent editing the submitted cart.
            form.querySelectorAll('input, select, textarea, button').forEach(function (control) { control.disabled = true; });
            var link = document.createElement('a');
            link.href = window.location.pathname;
            link.className = 'btn btn-primary';
            link.textContent = 'Tạo đơn hàng tiếp theo';
            el('om-save-status').appendChild(document.createTextNode(' '));
            el('om-save-status').appendChild(link);
            window.jQuery('.chosen-select').trigger('chosen:updated');
            return;
        }
        var cart = data.GETPROMOTION != null ? data.GETPROMOTION : data.ProductCombo;
        if (cart != null) el('tbodyTempItemInput').innerHTML = cart;
        if (data.MAPHIEU) el('MAPHIEU').value = data.MAPHIEU;
        if (data.SOPHIEU != null) el('SOPHIEU').value = data.SOPHIEU;
        if (typeof window.initializeMaskInputs === 'function') window.initializeMaskInputs();
        message('Đã cập nhật khuyến mãi. Vui lòng xem lại đơn hàng trước khi lưu.');
    };
    window.orderMenuFailed = function () { setBusy(false); message('Không nhận được kết quả lưu. Vui lòng kiểm tra danh sách phiếu trước khi thử lại.', true); };
    function customerChanged() {
        if (customerRequest) customerRequest.abort();
        el('ADDRESS').value = ''; el('TEL').value = ''; el('lblCongNo').textContent = '';
        if (!el('lstdm_KhachHang').value) { customerBusy = false; return; }
        customerBusy = true;
        customerRequest = window.jQuery.ajax({
            url: root.getAttribute('data-customer-url'), type: 'POST', dataType: 'json', data: { id: el('lstdm_KhachHang').value },
            success: function (data) {
                if (data.URL) { window.location.href = data.URL; return; }
                if (data.DataObject) {
                    el('ADDRESS').value = data.DataObject.ADDRESS || '';
                    el('TEL').value = data.DataObject.TEL || '';
                    el('lblCongNo').textContent = data.DataObject.CONGNOTHONGBAO || '';
                    customerBusy = false;
                    if (pendingProduct) selectProduct(pendingProduct.product, pendingProduct.button);
                } else { message('Không tải được thông tin khách hàng. Vui lòng chọn lại khách hàng.', true); }
            },
            error: function (xhr, status) { if (status !== 'abort') message('Không tải được thông tin khách hàng. Vui lòng chọn lại khách hàng.', true); },
            complete: function () { customerBusy = false; }
        });
    }
    function init() {
        root = el('orderMenuPage'); if (!root) return;
        form = el('orderMenuForm');
        selectedWarehouse = el('lstdm_Kho').value;
        selectedCustomer = el('lstdm_KhachHang').value;
        root.querySelectorAll('.om-mode').forEach(function (button) {
            button.onclick = function () {
                var mode = button.getAttribute('data-mode');
                el('om-products').className = 'om-products om-' + mode;
                root.querySelectorAll('.om-mode').forEach(function (item) {
                    var active = item === button;
                    item.classList.toggle('is-active', active);
                    item.setAttribute('aria-pressed', String(active));
                });
            };
        });
        el('om-search-button').onclick = loadProducts;
        el('om-search').onkeydown = function (event) { if (event.key === 'Enter' || event.keyCode === 13) { event.preventDefault(); loadProducts(); } };
        initGroups();
        el('om-include-empty').onchange = loadProducts;
        window.jQuery('#lstdm_Kho').on('change.orderMenu', function () {
            if (!retainCartScope('lstdm_Kho', selectedWarehouse)) return;
            selectedWarehouse = this.value;
            pendingProduct = null;
            loadProducts();
        });
        window.jQuery('#lstdm_KhachHang').on('change.orderMenu', function () {
            if (!retainCartScope('lstdm_KhachHang', selectedCustomer)) return;
            selectedCustomer = this.value;
            customerChanged();
        });
        el('om-prev').onclick = function () { if (page > 0) { page--; render(); } };
        el('om-next').onclick = function () { if ((page + 1) * pageSize < products.length) { page++; render(); } };
        new MutationObserver(function () {
            var hasItems = !!el('tbodyTempItemInput').querySelector('input[name^="txt"]');
            el('om-cart-empty').hidden = hasItems;
            // Warehouse and customer must stay consistent with the current cart.
            ['lstdm_Kho', 'lstdm_KhachHang'].forEach(function (id) {
                var chosen = el(id + '_chosen');
                if (chosen) chosen.style.pointerEvents = hasItems ? 'none' : '';
            });
        }).observe(el('tbodyTempItemInput'), { childList: true });
        loadProducts();
    }
    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init); else init();
}());
