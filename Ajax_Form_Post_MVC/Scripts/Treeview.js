function OnchangeCheckbox(e)
{
    const checkbox = e.target;
    var array = document.querySelectorAll('#treeview input[type="checkbox"]')
    if (array != null && array.length > 0)
    {
        for (var i = 0; i < array.length; i++)
        {
            array[i].checked = checkbox.checked;
        }
    }
}
function OnchangeCheckboxTBL_DEPT(e) {
    const checkbox = e.target;
    var array = document.querySelectorAll('#treeview input[type="checkbox"]')
    if (array != null && array.length > 0)
    {
        for (var i = 0; i < array.length; i++)
        {
            if (array[i].name =="TBL_DEPT")
                array[i].checked = checkbox.checked;
        }
    }
}

function SaveTree(actionName, controllerName, LOC_ID) {
    OpenLoader();
    var e = document.getElementById("idNhomQuyen");
    var value = e.options[e.selectedIndex].value;
    var cartList = [];
    var aElements = document.querySelectorAll('#treeview input[type="checkbox"]');
    for (var i = 0; i < aElements.length; i++) {
        if (aElements[i].type == 'checkbox') {
            cartList.push({
                id: aElements[i].id,
                Name: aElements[i].name,
                Checked: aElements[i].checked,
                idNhomQuyen: value,
                LOC_ID: LOC_ID,
                idNhomSanPham: aElements[i].dataset.id
            });
        }
    }

    $.ajax({
        type: "POST",
        url: "/" + controllerName + "/" + actionName,
        data: "{cartOrder:'" +  JSON.stringify(cartList) + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccessAddProduct,
        error: myFunClosed
    });
}

function SaveTreeCustomer(actionName, controllerName, LOC_ID) {
    OpenLoader();
    var e = document.getElementById("idNhomQuyen");
    var value = e.options[e.selectedIndex].value;

    e = document.getElementById("idLichLamViec");
    var value1 = e.options[e.selectedIndex].value;

    var cartList = [];
    var aElements = document.querySelectorAll('#treeview input[type="checkbox"]');
    for (var i = 0; i < aElements.length; i++) {
        if (aElements[i].type == 'checkbox') {
            cartList.push({
                id: aElements[i].id,
                Name: aElements[i].name,
                Checked: aElements[i].checked,
                idNhomQuyen: value,
                LOC_ID: LOC_ID,
                idNhomSanPham: aElements[i].dataset.id,
                idLichLamViec: value1
            });
        }
    }

    $.ajax({
        type: "POST",
        url: "/" + controllerName + "/" + actionName,
        data: "{cartOrder:'" + JSON.stringify(cartList) + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccessAddProduct,
        error: myFunClosed
    });
}

$(document).ready(function () {
    $("#SearchStringTree").on("keyup", function (event) {
        if (event.keyCode == 13) {
            funSearchTreeview();
        }
    });
});

function funSearchTreeview() {
    OpenLoader();
    var searchTerm = normalizeTreeSearchText(document.getElementById("SearchStringTree").value, true);
    var items = document.getElementsByClassName("licbx");
    for (var j = 0; j < items.length; j++) {
        var itemText = normalizeTreeSearchText(items[j].innerText, false);
        if (itemText.indexOf(searchTerm) > -1)
            items[j].classList.value = "licbx active1";
        else
            items[j].classList.value = "licbx nested1";
    }
    var toggler = document.getElementsByClassName("caret1");

    for (var i = 0; i < toggler.length; i++) {
        if (toggler[i].classList.value.indexOf("caret1-down") < 0) {
            toggler[i].parentElement.querySelector(".nested").classList.toggle("active");
            toggler[i].classList.toggle("caret1-down");
        }
        else {
            if (searchTerm.length == 0) {
                toggler[i].classList.value = "caret1";
                toggler[i].parentElement.querySelector(".nested").classList.value = "nested";
            }
        }
    }
    CloseLoader();
}

var treeVietnameseAccentMap = {
    "à": "a", "á": "a", "ạ": "a", "ả": "a", "ã": "a", "â": "a", "ầ": "a", "ấ": "a", "ậ": "a", "ẩ": "a", "ẫ": "a", "ă": "a", "ằ": "a", "ắ": "a", "ặ": "a", "ẳ": "a", "ẵ": "a",
    "è": "e", "é": "e", "ẹ": "e", "ẻ": "e", "ẽ": "e", "ê": "e", "ề": "e", "ế": "e", "ệ": "e", "ể": "e", "ễ": "e",
    "ì": "i", "í": "i", "ị": "i", "ỉ": "i", "ĩ": "i",
    "ò": "o", "ó": "o", "ọ": "o", "ỏ": "o", "õ": "o", "ô": "o", "ồ": "o", "ố": "o", "ộ": "o", "ổ": "o", "ỗ": "o", "ơ": "o", "ờ": "o", "ớ": "o", "ợ": "o", "ở": "o", "ỡ": "o",
    "ù": "u", "ú": "u", "ụ": "u", "ủ": "u", "ũ": "u", "ư": "u", "ừ": "u", "ứ": "u", "ự": "u", "ử": "u", "ữ": "u",
    "ỳ": "y", "ý": "y", "ỵ": "y", "ỷ": "y", "ỹ": "y", "đ": "d"
};

function normalizeTreeSearchText(value, trimWhitespace) {
    var text = (value || "").toLowerCase();

    if (trimWhitespace) {
        text = text.trim();
    }

    return text.replace(/[àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]/g, function (character) {
        return treeVietnameseAccentMap[character] || character;
    });
}

function OpenLoader() {
    var modal = document.getElementById("myModal1");
    modal.style.display = "block";
    //var toggler = document.getElementById("Loadpreloader");
    //toggler.innerHTML = " <div id=\"preloader\"><div id=\"status\"> <i class=\"fa fa-spinner fa-spin\"></i></div></div>";
}
function CloseLoader() {
    var modal = document.getElementById("myModal1");
    modal.style.display = "none";
    //var toggler1 = document.getElementById("Loadpreloader");
    //toggler1.innerHTML = "";
}
function myFunClosed() {
    alert("Phát sinh lỗi!");
    CloseLoader();
}
function OnSuccessAddProduct(data) {
    alert(data);
    CloseLoader();
}
$(function () {

    $('input.cbx').change(checkboxChanged);

    function checkboxChanged() {
        var $this = $(this),
            checked = $this.prop("checked"),
            container = $this.parent(),
            siblings = container.siblings();

        container.find('input.cbx')
            .prop("checked", checked)
            .siblings('label')
            .removeClass('custom-checked custom-unchecked custom-indeterminate')
            .addClass(checked ? 'custom-checked' : 'custom-unchecked');

        checkSiblings(container, checked);
    }

    function checkSiblings($el, checked) {
        var parent = $el.parent().parent(),
            all = true,
            indeterminate = false;

        //$el.siblings().each(function () {
        //    return all = ($(this).children('input.cbx').prop("checked") === checked);
        //});

        if (all && checked) {
            parent.children('input.cbx')
                .prop("checked", checked)
                .siblings('label')
                .removeClass('custom-checked custom-unchecked custom-indeterminate')
                .addClass(checked ? 'custom-checked' : 'custom-unchecked');

            checkSiblings(parent, checked);
        }
        else if (all && !checked) {
            indeterminate = parent.find('input.cbx:checked').length > 0;

            parent.children('input.cbx')
                .prop("checked", true)
                .siblings('label')
                .removeClass('custom-checked custom-unchecked custom-indeterminate')
                .addClass(indeterminate ? 'custom-checked' : 'custom-unchecked');

            checkSiblings(parent, checked);
        }
        else {
            $el.parents("li").children('input.cbx')
                .prop({
                    indeterminate: true,
                    checked: true
                })
                .siblings('label')
                .removeClass('custom-checked custom-unchecked custom-indeterminate')
                .addClass('custom-checked');
        }
    }
});


