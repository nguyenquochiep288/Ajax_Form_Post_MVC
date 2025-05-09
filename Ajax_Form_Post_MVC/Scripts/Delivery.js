function myFunctionDelivery(Controller, Class, ID)
{
    $.ajax({
        type: "GET",
        url: "/" + Controller + "/" + Class + "?ID=" + ID,
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccessLoadDelivery,
        error: OnErrorLoadEdit
    });
}

function OnSuccessLoadDelivery(apiResponse) {
    try {
        
        if (apiResponse.Success)
        {
            for (var i = 0; i < apiResponse.Detail.length; i++)
            {
                var divElem = document.getElementById(apiResponse.Detail[i].Key);
                if (divElem != null) {
                    divElem.innerHTML = apiResponse.Detail[i].Value;
                    LoadTree_table();
                }
            }
        }
        else {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else
                alert(apiResponse.Message);
        }
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }

}

function OnSubmitDelivery() {
    try {
        var cartList = [];
        var array = document.querySelectorAll('#tbodySearchDelivery input[name="TBL_ITEM"]')
        if (array != null && array.length > 0) {
            for (var i = 0; i < array.length; i++) {
                if (array[i].checked == true) {
                    cartList.push({
                        ID: array[i].id
                    });
                }
            }
            if (cartList != null && cartList.length > 0) {
                $.ajax({
                    url: "/Delivery/AddDeliveryDetail",
                    data: { cartOrder: JSON.stringify(cartList) },
                    dataType: "json",
                    success: OnSuccessLoadDelivery,
                    error: OnErrorLoadEdit
                });
            }
            else {
                alert("Vui lòng chọn ít nhất 1 phiếu!");
            }
            CloseLoaderDelivery();
        }
    }
    catch (ex) {
        alert(ex);
    }
}

function OnErrorLoadEdit()
{
    alert("Lỗi: vui lòng liên hệ nhà cung cấp! Xin cảm ơn. Deleivery");
}

function myFunOpenSearchDelivery() {
    OpenLoaderDelivery();
}

function OpenLoaderDelivery() {
    try {
        var modal = document.getElementById("myModalSearchDelivery");
        modal.style.display = "block";
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}

function CloseLoaderDelivery() {
    try {
        var modal = document.getElementById("myModalSearchDelivery");
        modal.style.display = "none";
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}

function LoadTree_table() {
    var
        $table = $('#tree-table'),
        rows = $table.find('tr');

    rows.each(function (index, row) {
        var
            $row = $(row),
            level = $row.data('level'),
            id = $row.data('id'),
            $columnName = $row.find('td[data-column="name"]'),
            children = $table.find('tr[data-parent="' + id + '"]');

        if (children.length) {
            var expander = $columnName.prepend('' +
                '<span class="treegrid-expander glyphicon glyphicon-chevron-right"></span>' +
                '');

            children.hide();

            expander.on('click', function (e) {
                var $target = $(e.target);
                if ($target.hasClass('glyphicon-chevron-right')) {
                    $target
                        .removeClass('glyphicon-chevron-right')
                        .addClass('glyphicon-chevron-down');

                    children.show();
                } else {
                    $target
                        .removeClass('glyphicon-chevron-down')
                        .addClass('glyphicon-chevron-right');

                    reverseHide($table, $row);
                }
            });
        }

        $columnName.prepend('' +
            '<span class="treegrid-indent" style="width:' + 15 * level + 'px"></span>' +
            '');
    });

    // Reverse hide all elements
    reverseHide = function (table, element) {
        var
            $element = $(element),
            id = $element.data('id'),
            children = table.find('tr[data-parent="' + id + '"]');

        if (children.length) {
            children.each(function (i, e) {
                reverseHide(table, e);
            });

            $element
                .find('.glyphicon-chevron-down')
                .removeClass('glyphicon-chevron-down')
                .addClass('glyphicon-chevron-right');

            children.hide();
        }
    };
}

