
function myInputOnkeyup(Class, event)
{
    var value = document.getElementById("myInput").value;
    var current = document.getElementsByClassName("btnGroup active");
    if (event.which == 13) {
        myFunctionPage(current[0].id, value.toLowerCase(), Class);
    }
}

function myFunClosedProduct() {
    CloseLoader();
    var modal = document.getElementById("myModalDeposit_Temp");
    modal.style.display = "none";
}

function OpenLoader() {
    var modal = document.getElementById("myModal1");
    modal.style.display = "block";
}

function CloseLoaderDeposit(myModal) {
    var modal = document.getElementById(myModal);
    modal.style.display = "none";
}

function OpenLoaderDeposit(myModal) {
    var modal = document.getElementById(myModal);
    modal.style.display = "block";
}

function CloseLoader() {
    try
    {
        var modal = document.getElementById("myModal1");
        modal.style.display = "none";
    }
    catch
    { }
}

function myFunOpenProduct(obj, IDProduct)
{
    OpenLoader();
    if (obj != null) { 
        //var current = document.getElementsByClassName("filterDiv btnItem active show");
        //if (current.length > 0) {
        //    current[0].className = current[0].className.replace("filterDiv btnItem active show", "filterDiv active show");
        //}
        //obj.className = "filterDiv btnItem active show";

        const buttons = document.querySelectorAll('.productDeposit-button');
        buttons.forEach(btn => btn.classList.remove('activeDeposit'));

        // Add 'active' class to the clicked button
        obj.classList.add('activeDeposit');
    }
   
    $.ajax({
        type: "POST",
        url: "/Deposit/LoadProduct",
        data: "{id:'" + IDProduct + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccessDeposit_Temp,
        error: OnError
    });
}
function CallChangeCustomer(val)
{
    if (val != null && val != '') {
        OpenLoader();
        $.ajax({
            type: "POST",
            url: "/Deposit/CallChangeCustomer",
            data: "{id:'" + val + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessCallChangeCustomer,
            error: OnError
        });
    }
    else {
        var x = document.getElementsByName("ADDRESS");
        if (x != null && x.length > 0) {
            for (var i = 0; i < x.length; i++) {
                x[i].value = '';
            }
        }
       
        x = document.getElementsByName("TEL");
        if (x != null && x.length > 0) {
            for (var i = 0; i < x.length; i++) {
                x[i].value = '';
            }
        }

        x = document.getElementsByName("lblCongNo");
        if (x != null && x.length > 0) {
            for (var i = 0; i < x.length; i++) {
                x[i].innerHTML = "";
            }
        }

        x = document.getElementsByName("LATITUDE");
        if (x != null && x.length > 0) {
            for (var i = 0; i < x.length; i++) {
                x[i].value = null;
            }
        }

        x = document.getElementsByName("LONGITUDE");
        if (x != null && x.length > 0) {
            for (var i = 0; i < x.length; i++) {
                x[i].value = null;
            }
        }

        x = document.getElementsByName("demoMap");
        if (x != null && x.length > 0) {
            for (var i = 0; i < x.length; i++) {
                x[i].innerHTML = "";
            }
        }
        document.getElementById("btnGetLocation").style.visibility = "hidden";
        document.getElementById("btnSave_Map").style.visibility = "hidden";
        document.getElementById("btnOpenMap").style.visibility = "hidden";
    }

    //myFunctionPage("all");
    //LoadFun();
    //myLoadProduct_Detail();
}

function OnSuccessCallChangeCustomer(data)
{
    CloseLoader();
    if (data != null && data.DataObject != null)
    {
        if (data.KHONGDUOCPHEPTAO == true)
        {
            alert("Cảnh báo " + data.DataObject.CONGNOTHONGBAO);
        }
        var x = document.getElementsByName("ADDRESS");
        if (x != null && x.length > 0) {
            for (var i = 0; i < x.length; i++) {
                x[i].value = data.DataObject.ADDRESS;
            }
        }

        x = document.getElementsByName("TEL");
        if (x != null && x.length > 0) {
            for (var i = 0; i < x.length; i++) {
                x[i].value = data.DataObject.TEL;
            }
        }

        x = document.getElementsByName("lblCongNo");
        if (x != null && x.length > 0) {
            for (var i = 0; i < x.length; i++) {
                x[i].innerHTML = data.DataObject.CONGNOTHONGBAO;
            }
        }

        x = document.getElementsByName("LATITUDE");
        if (x != null && x.length > 0) {
            for (var i = 0; i < x.length; i++) {
                x[i].value = data.DataObject.LATITUDE;
            }
        }

        x = document.getElementsByName("LONGITUDE");
        if (x != null && x.length > 0) {
            for (var i = 0; i < x.length; i++) {
                x[i].value = data.DataObject.LONGITUDE;
            }
        }

        x = document.getElementsByName("demoMap");
        if (x != null && x.length > 0) {
            for (var i = 0; i < x.length; i++) {
                x[i].innerHTML = data.DataObject.CONTENT_MAP;
            }
        }

        document.getElementById("btnGetLocation").style.visibility = "";
        document.getElementById("btnSave_Map").style.visibility = "";
        document.getElementById("btnOpenMap").style.visibility = "";
        var lst = document.querySelectorAll("span");
        if (lst != null) {
            for (var i = 0; i < lst.length; i++) {
                if (lst[i].className == "field-validation-valid text-danger") {
                    lst[i].innerHTML = "";
                }
            }
        }
    }
    else if (data.URL != null && data.URL != "")
            window.location.href = data.URL;
}

function myResetProduct_DetailList() {
    OpenLoader();
    $.ajax({
        type: "POST",
        url: "/Deposit/ResetProduct_DetailList",
        success: OnSuccessResetProduct_DetailList,
        error: myFunClosedProduct
    });
}
function OnSuccessResetProduct_DetailList(data) {
    try {
        if (data.URL != null && data.URL != "")
            window.location.href = data.URL;
        else
            document.getElementById("ShowProdcut").innerHTML = data.DATA;
        CloseLoader();
    }
    catch
    {
        CloseLoader();
    }

}
function myLoadProduct_Detail() {
    OpenLoader();
    $.ajax({
        type: "POST",
        url: "/Deposit/LoadProduct_Detail",
        success: OnSuccessAddProduct,
        error: myFunClosedProduct
    });
}
function myUpdateProduct_Detail() {
    try
    { 
        OpenLoader();
        var listProduct = $('.txtQuantity');
        var cartList = [];
        $.each(listProduct, function (i, item) {
            cartList.push({
                QTY: $(item).val(),
                ID_ITEM: $(item).data('id')
            });
        });

        $.ajax({
            url: "/Deposit/UpdateProduct_Detail",
            data: { cartDeposit_Temp: JSON.stringify(cartList) },
            dataType: "json",
            success: OnSuccessAddProduct,
            error: CloseLoader
        });
    }
    catch (ex)
    {
        alert(ex);
    }
}

function myAddProduct_Detail()
{
    try
    {
        OpenLoader();
        var idProduct = document.getElementById("idProduct").value;
        var QtyProduct = document.getElementById("QtyProduct").value;
        $.ajax({
            type: "POST",
            url: "/Deposit/AddProduct_Detail",
            data: "{ID_ITEM:'" + idProduct + "',QTY:" + QtyProduct + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessAddProduct,
            error: myFunClosedProduct
        });
    }
    catch (ex)
    {
        alert(ex);
    }
}

function myAddProduct_DetailList() {
    try
    { 
        OpenLoader();
        var e = document.getElementById("productCode");
        //var idProduct = e.options[e.selectedIndex].value;
        var idProduct = e.value;
        $.ajax({
            type: "POST",
            url: "/Deposit/AddProduct_Detail",
            data: "{ID_ITEM:'" + idProduct + "',QTY:1}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessAddProduct,
            error: CloseLoader
        });
    }
    catch (ex) {
        alert(ex);
    }
}

function myFunctionPage(GroupID, key, Class)
{
    try
    { 
        var current = document.getElementsByClassName("btnGroup");
        if (current != null)
        {
             for (var i = 0; i < current.length; i++)
                {
                 if (current[i].id == GroupID)
                     current[i].classList.value = "btnGroup active";
                 else
                     current[i].classList.value = "btnGroup";     
                }
        }

        var divElem = document.getElementById(Class);
        var lst = divElem.querySelectorAll("select");
        var ID_KHO = "";
        if (lst != null && lst.length > 0) {
            for (var j = 0; j < lst.length; j++) {
                if (lst[j].name == "ID_KHO") {
                    ID_KHO = lst[j].value;
                    if (ID_KHO == "") {
                        alert("Vui lòng chọn kho cần làm việc!");
                        return;
                    }
                    break;
                }

            }
        }
        OpenLoader();
        var employee = new Object();
        employee.GroupID = GroupID;
        employee.keySearch = key;
        employee.ID_KHO = ID_KHO;
        $.ajax({
            type: "POST",
            url: "/Deposit/LoadDanhSachSanPham",
            data: JSON.stringify(employee),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccess,
            error: OnError
        });
    }
    catch (ex) {
        alert(ex);
        CloseLoader();
    }
}

function LoadFun() {
    // Add active class to the current button (highlight it)
    var btnContainer = document.getElementById("myBtnContainer");
    var btns = btnContainer.getElementsByClassName("btnGroup");
    for (var i = 0; i < btns.length; i++) {
        btns[i].addEventListener("click", function () {
            var current = document.getElementsByClassName("btnGroup active");
            current[0].className = current[0].className.replace(" active", "");
            this.className = "btnGroup active";
        });
    }
}
function myFunctionLoadGroup(Class) {
    OpenLoader();
    $.ajax({
        type: "POST",
        url: "/Deposit/LoadGroup",
        data: "{Class:'" + Class + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccessLoadGroup,
        error: OnError
    });
}
function OnSuccessLoadGroup(data) {
    try {
        if (data.URL != null && data.URL != "")
            window.location.href = data.URL;
        else {
            $("#myBtnContainer").html(data.DATA);
            myFunctionPage("all", "", data.CHUOIPHANTRANG)
            LoadFun();
        }
        CloseLoader();
    }
    catch {
        CloseLoader();
    }
}

function OnSuccess(data) {
    try { 
        if (data.URL != null && data.URL != "")
        { window.location.href = data.URL; }
        else
        {
            $("#mycontainer").html(data.DATA);
        }

        CloseLoader();
    }
    catch {
        CloseLoader();
    }
}

function OnError(data) {
    alert("Lỗi: vui lòng liên hệ! Xin cảm ơn.");
    CloseLoader();
}

function filterSelection(c) {
    var x, i;
    x = document.getElementsByClassName("productDeposit");
    if (c == "all") c = "";
    for (i = 0; i < x.length; i++) {
        w3RemoveClass(x[i], "show");
        if (x[i].className.indexOf(c) > -1) w3AddClass(x[i], "show");
    }
}

function w3AddClass(element, name) {
    var i, arr1, arr2;
    arr1 = element.className.split(" ");
    arr2 = name.split(" ");
    for (i = 0; i < arr2.length; i++) {
        if (arr1.indexOf(arr2[i]) == -1) { element.className += " " + arr2[i]; }
    }
}

function w3RemoveClass(element, name) {
    var i, arr1, arr2;
    arr1 = element.className.split(" ");
    arr2 = name.split(" ");
    for (i = 0; i < arr2.length; i++) {
        while (arr1.indexOf(arr2[i]) > -1) {
            arr1.splice(arr1.indexOf(arr2[i]), 1);
        }
    }
    element.className = arr1.join(" ");
}

function funSearchItemProduct(CLass)
{
    var value = document.getElementById("myInput").value;
    var current = document.getElementsByClassName("btnGroup active");
    myFunctionPage(current[0].id, value, CLass);
}

function OnSuccessDeposit_Temp(apiResponse) {
    try {
        var modal = document.getElementById("myModalDeposit_Temp");
        modal.style.display = "block";
        if (apiResponse.Success) {
            var radiovalue = "";
            var nf = Intl.NumberFormat('vi-VN');
            for (var m = 0; m < apiResponse.Detail.length; m++) {
                if (apiResponse.Detail[m].Key == "NAME") {
                    document.getElementById("lblNameDeposit_Temp").innerHTML = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_HANGHOAKHO") {
                    document.getElementById("idProductKhoDeposit_Temp").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_HANGHOA") {
                    document.getElementById("idProductDeposit_Temp").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_KHO") {
                    document.getElementById("idDepotDeposit_Temp").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_DVT") {
                    document.getElementById("ID_DVTDeposit_Temp").value = apiResponse.Detail[m].Value;
                    radiovalue = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_DVT_QD") {
                    if (apiResponse.Detail[m].Value != null && apiResponse.Detail[m].Value != "") {
                        document.getElementById("ID_DVT_QDDeposit_Temp").value = apiResponse.Detail[m].Value;
                        document.getElementById("ID_DVT_QDDeposit_Temp").style.display = "";
                    }
                    else {
                        document.getElementById("ID_DVT_QDDeposit_Temp").style.display = "none";
                        document.getElementById("ID_DVT_QDDeposit_Temp").value = "";
                    }
                    continue;
                }
                if (apiResponse.Detail[m].Key == "NAME_DVT") {
                    document.getElementById("lblID_DVTDeposit_Temp").innerHTML = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "NAME_DVT_QD") {
                    if (apiResponse.Detail[m].Value != null && apiResponse.Detail[m].Value != "") {
                        document.getElementById("ID_DVT_QDDeposit_Temp").style.display = "";
                        document.getElementById("lblID_DVT_QDDeposit_Temp").innerHTML = apiResponse.Detail[m].Value;
                    }
                    else {
                        document.getElementById("ID_DVT_QDDeposit_Temp").style.display = "none";
                        document.getElementById("lblID_DVT_QDDeposit_Temp").innerHTML = "";
                    }

                    continue;
                }
                if (apiResponse.Detail[m].Key == "GIA") {
                    document.getElementById("PriceDeposit_Temp").value = apiResponse.Detail[m].Value;
                    document.getElementById("PriceProductDeposit_Temp").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "GIA_QD") {
                    document.getElementById("Price_QDDeposit_Temp").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_THUESUAT") {
                    document.getElementById("lstdm_ThueSuatDeposit").value = apiResponse.Detail[m].Value;
                    jQuery("#lstdm_ThueSuatDeposit").val(apiResponse.Detail[m].Value);
                    jQuery("#lstdm_ThueSuatDeposit").trigger("chosen:updated");
                    //$("#lstdm_ThueSuatSearch").val(apiResponse.Detail[m].Value).change();
                    continue;
                }
                if (apiResponse.Detail[m].Key == "THUESUAT") {
                    document.getElementById("THUESUATProductDeposit_Temp").value = apiResponse.Detail[m].Value;
                    continue;
                }
            }
            document.getElementById("DiscountProductDeposit_Temp").value = 0;
            document.getElementById("PriceDiscountProductDeposit_Temp").value = 0;
            $('input:radio[name="ID_DVT"][value=\"' + radiovalue + '\"]').prop('checked', true);
            document.getElementById("QtyProductDeposit_Temp").value = 1;
            updateAddProduct(document.getElementById("DiscountProductDeposit_Temp"));

            var modal = document.getElementById("myModalDeposit_Temp");
            modal.style.display = "block";

            $("input.form-control.mask").each((i, ele) => {
                let ele1 = $(ele)
                if (ele.type == "number") {
                    $(ele1).hide()
                }
                else {
                    $(ele1).show()
                }
            })
        }
        else {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else
                alert(apiResponse.Message);
        }
        CloseLoader();
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoader();
    }
}

function OnSuccessProductDeposit_Temp(apiResponse) {
    try {
        if (apiResponse.Success) {
            try {
                var tbodyTempItemCombo = document.getElementById('tbodyTempItemInput');
                tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
                LockKho("lstdm_Kho", apiResponse.ProductCombo);
            } catch { }

            try {
                var tbodyTempItemCombo = document.getElementById('tbodyTempItemInputEdit');
                tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
                LockKho("lstdm_KhoEdit", apiResponse.ProductCombo);
            } catch { }


            $("input.form-control.maskinput").each((i, ele) => {
                let clone = $(ele).clone(false)
                clone.attr("type", "text")
                let ele1 = $(ele)
                clone.val(Number(ele1.val()).toLocaleString("vn"))

                $(ele).after(clone)
                $(ele).hide()
                clone.mouseenter(() => {

                    ele1.show()
                    clone.hide()
                })
                setInterval(() => {
                    var newv = Number(ele1.val()).toLocaleString("vn");
                    if (clone.val() != newv) {
                        clone.val(newv)
                    }
                }, 10)

                $(ele).mouseleave(() => {
                    $(clone).show()
                    $(ele1).hide()
                })


            })
            myFunClosed("myModalDeposit_Temp");
        }
        else {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else
                alert(apiResponse.Message);
        }
        CloseLoader();
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoader();
    }
}

function myFunctionDeleteProdcutDeposit_Temp(Controller, id) {
    if (confirm("Bạn muốn thực hiện xóa!")) {
        $.ajax({
            type: "POST",
            url: "/Deposit/DeleteProductDeposit_Temp",
            data: "{ID:'" + id + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessProductDeposit_Temp
        });
    }
}

function updateAddProduct(elem) {
    try {
        var ID_HANGHOAKHO = document.getElementById("idProductKhoDeposit_Temp").value;
        var ID_HANGHOA = document.getElementById("idProductDeposit_Temp").value;
        var ID_DVT = document.getElementById("ID_DVTDeposit_Temp").value;
        var SOLUONG = document.getElementById("QtyProductDeposit_Temp").value;
        var DONGIA = document.getElementById("PriceProductDeposit_Temp").value;
        var CHIETKHAU = document.getElementById("DiscountProductDeposit_Temp").value;
        var TONGTIENGIAMGIA = document.getElementById("PriceDiscountProductDeposit_Temp").value;
        var THANHTIEN = document.getElementById("IntoMoneyProductDeposit_Temp").value;
        var ID_THUESUAT = document.getElementById("lstdm_ThueSuatDeposit").value;
        var TONGTIENVAT = document.getElementById("TaxMoneyProductDeposit_Temp").value;
        var TONGCONG = document.getElementById("TotalProductDeposit_Temp").value;
        var THUESUAT = document.getElementById("THUESUATProductDeposit_Temp").value;
        var ID_KHO = document.getElementById("idDepotDeposit_Temp").value;
        $.post("/Deposit/UpdateAddProduct", { "ID_KHO": ID_KHO, "TYPE": elem.name, "ID_HANGHOA": ID_HANGHOA, "ID_HANGHOAKHO": ID_HANGHOAKHO, "ID_DVT": ID_DVT, "SOLUONG": SOLUONG, "DONGIA": DONGIA, "CHIETKHAU": CHIETKHAU, "TONGTIENGIAMGIA": TONGTIENGIAMGIA, "THANHTIEN": THANHTIEN, "THUESUAT": THUESUAT, "ID_THUESUAT": ID_THUESUAT, "TONGTIENVAT": TONGTIENVAT, "TONGCONG": TONGCONG }, function (data) {
            document.getElementById("idProductKhoDeposit_Temp").value = data.Detail.ID_HANGHOAKHO;
            document.getElementById("idProductDeposit_Temp").value = data.Detail.ID_HANGHOA;
            document.getElementById("ID_DVTDeposit_Temp").value = data.Detail.ID_DVT;
            document.getElementById("QtyProductDeposit_Temp").value = data.Detail.SOLUONG;
            document.getElementById("PriceProductDeposit_Temp").value = data.Detail.DONGIA;
            document.getElementById("DiscountProductDeposit_Temp").value = data.Detail.CHIETKHAU;
            document.getElementById("PriceDiscountProductDeposit_Temp").value = data.Detail.TONGTIENGIAMGIA;
            document.getElementById("IntoMoneyProductDeposit_Temp").value = data.Detail.THANHTIEN;
            document.getElementById("lstdm_ThueSuatDeposit").value = data.Detail.ID_THUESUAT;
            document.getElementById("TaxMoneyProductDeposit_Temp").value = data.Detail.TONGTIENVAT;
            document.getElementById("TotalProductDeposit_Temp").value = data.Detail.TONGCONG;
            document.getElementById("THUESUATProductDeposit_Temp").value = data.Detail.THUESUAT;
            document.getElementById("idDepotDeposit_Temp").value = data.Detail.ID_KHO;
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }

}

function myDeleteAllDeposit_TempProduct() { 
    if (confirm("Bạn muốn thực hiện xóa tất cả!")) {
        $.post("/Deposit/DeleteAllProductDeposit_Temp", null, function (apiResponse) {
            try {
                if (apiResponse.Success) {

                    try {
                        var tbodyTempItemCombo = document.getElementById('tbodyTempItemInput');
                        tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
                        LockKho("lstdm_Kho", apiResponse.ProductCombo);
                    } catch { }

                    try {
                        var tbodyTempItemCombo = document.getElementById('tbodyTempItemInputEdit');
                        tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
                        LockKho("lstdm_KhoEdit", apiResponse.ProductCombo);
                    } catch { }

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
        });
    }
}
function updateDeposit_Temp(ID, elem) {

    $.post("/Deposit/UpdateProductDeposit_Temp", { "ID": ID, "TYPE": elem.id, "VALUE": elem.value }, function (apiResponse) {
        try {
            if (apiResponse.Success) {

                try {
                    var tbodyTempItemCombo = document.getElementById('tbodyTempItemInput');
                    tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
                    LockKho("lstdm_Kho", apiResponse.ProductCombo);
                } catch { }

                try {
                    var tbodyTempItemCombo = document.getElementById('tbodyTempItemInputEdit');
                    tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
                    LockKho("lstdm_KhoEdit", apiResponse.ProductCombo);
                } catch { }

                $("input.form-control.maskinput").each((i, ele) => {
                    let clone = $(ele).clone(false)
                    clone.attr("type", "text")
                    let ele1 = $(ele)
                    clone.val(Number(ele1.val()).toLocaleString("vn"))

                    $(ele).after(clone)
                    $(ele).hide()
                    clone.mouseenter(() => {

                        ele1.show()
                        clone.hide()
                    })
                    setInterval(() => {
                        var newv = Number(ele1.val()).toLocaleString("vn");
                        if (clone.val() != newv) {
                            clone.val(newv)
                        }
                    }, 10)

                    $(ele).mouseleave(() => {
                        $(clone).show()
                        $(ele1).hide()
                    })
                })
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
    });
}

function LockKho(name, value) {
    if (value != "") {
        document.getElementById(name + "_chosen").style.pointerEvents = "none";
    }
    else {
        document.getElementById(name + "_chosen").style.pointerEvents = "";
    }
}

function OnSuccessCreateDeposit(apiResponse) {
    try
    {
        CloseLoader();
        if (apiResponse.Success)
        {
            var x = document.getElementsByName("ID");
            if (x != null && x.length > 0) {
                for (var i = 0; i < x.length; i++) {
                    x[i].value = apiResponse.NewID;
                }
            }
            var xMP = document.getElementsByName("MAPHIEU");
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].value = apiResponse.MAPHIEU;
                }
            }

            var xSP = document.getElementsByName("SOPHIEU");
            if (xSP != null && xSP.length > 0) {
                for (var i = 0; i < xSP.length; i++) {
                    xSP[i].value = apiResponse.SOPHIEU;
                }
            }
            if (apiResponse.GETPROMOTION != null) {
                var tbodyTempItemInput = document.getElementById("tbodyTempItemInput");
                tbodyTempItemInput.innerHTML = apiResponse.GETPROMOTION;
                $("input.form-control.maskinput").each((ii, ele) => {
                    let clone = $(ele).clone(false)
                    clone.attr("type", "text")
                    let ele1 = $(ele)
                    clone.val(Number(ele1.val()).toLocaleString("vn"))

                    $(ele).after(clone)
                    $(ele).hide()
                    clone.mouseenter(() => {

                        ele1.show()
                        clone.hide()
                    })
                    setInterval(() => {
                        var newv = Number(ele1.val()).toLocaleString("vn");
                        if (clone.val() != newv) {
                            clone.val(newv)
                        }
                    }, 10)

                    $(ele).mouseleave(() => {
                        $(clone).show()
                        $(ele1).hide()
                    })
                })
            }
            else {
                if (confirm("Tạo thành công phiếu đặt hàng! ")) {
                    location.reload();
                }
            }
        }
        else
        {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else {
                if (apiResponse.CheckValue) {
                    var x = document.getElementsByName("ID");
                    x.value = apiResponse.NewID;
                    var xMP = document.getElementsByName("MAPHIEU");
                    if (xMP != null && xMP.length > 0) {
                        for (var i = 0; i < xMP.length; i++) {
                            xMP[i].value = apiResponse.MAPHIEU;
                        }
                    }
                    var xSP = document.getElementsByName("SOPHIEU");
                    if (xSP != null && xSP.length > 0) {
                        for (var i = 0; i < xSP.length; i++) {
                            xSP[i].value = apiResponse.SOPHIEU;
                        }
                    }
                    x = document.querySelectorAll(".validation-summary-errors");
                    if (x != null && x.length > 0) {
                        for (var i = 0; i < x.length; i++) {
                            gritter("Thông báo lỗi", apiResponse.Message);
                            x[i].innerHTML = "<ul><li>" + apiResponse.Message + "</li></ul>";
                        }
                    }
                    else {
                        gritter("Thông báo lỗi", apiResponse.Message);
                        x.innerHTML = "<ul><li>" + apiResponse.Message + "</li></ul>";
                    }


                }
                else {
                    if (apiResponse.Data != null) {
                        for (var i = 0; i < apiResponse.Data.length; i++) {
                            var text = "span[data-valmsg-for=\"" + apiResponse.Data[i].Key + "\"]";

                            if (apiResponse.Data[i].Error != null) {
                                $(text).text(apiResponse.Data[i].Error);
                                gritter("Thông báo lỗi", apiResponse.Data[i].Error);
                            }
                            else
                                $(text).text("");
                        }
                    }
                    var ValidationSummary = document.querySelectorAll(".validation-summary-errors");
                    if (ValidationSummary != null && ValidationSummary.length > 0) {
                        for (var i = 0; i < ValidationSummary.length; i++) {
                            gritter("Thông báo lỗi", apiResponse.Message);
                            ValidationSummary[i].innerHTML = "<ul><li>" + apiResponse.Message + "</li></ul>";
                        }
                    }
                    else {
                        gritter("Thông báo lỗi", apiResponse.Message);
                        ValidationSummary.innerHTML = "<ul><li>" + apiResponse.Message + "</li></ul>";
                    }
                }
            }
        }
       
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoader();
    }
}

function OnSuccessEditDeposit(apiResponse) {
    try {
        CloseLoader();
        if (apiResponse.Success) {
            if (apiResponse.URL != null && apiResponse.URL != "") {
                window.location.href = apiResponse.URL;
               
            }
            else
            {
                
                
                if (apiResponse.GETPROMOTION != null) {
                    var tbodyTempItemInput = document.getElementById("tbodyTempItemInputEdit");
                    tbodyTempItemInput.innerHTML = apiResponse.GETPROMOTION;
                    $("input.form-control.maskinput").each((ii, ele) => {
                        let clone = $(ele).clone(false)
                        clone.attr("type", "text")
                        let ele1 = $(ele)
                        clone.val(Number(ele1.val()).toLocaleString("vn"))

                        $(ele).after(clone)
                        $(ele).hide()
                        clone.mouseenter(() => {

                            ele1.show()
                            clone.hide()
                        })
                        setInterval(() => {
                            var newv = Number(ele1.val()).toLocaleString("vn");
                            if (clone.val() != newv) {
                                clone.val(newv)
                            }
                        }, 10)

                        $(ele).mouseleave(() => {
                            $(clone).show()
                            $(ele1).hide()
                        })
                    })
                }
            }
        }
        else {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else {
                if (apiResponse.CheckValue) {
                    x = document.querySelectorAll(".validation-summary-errors");
                    if (x != null && x.length > 0) {
                        for (var i = 0; i < x.length; i++) {
                            gritter("Thông báo lỗi", apiResponse.Message);
                            x[i].innerHTML = "<ul><li>" + apiResponse.Message + "</li></ul>";
                        }
                    }
                    else {
                        gritter("Thông báo lỗi", apiResponse.Message);
                        x.innerHTML = "<ul><li>" + apiResponse.Message + "</li></ul>";
                    }


                }
                else {
                    if (apiResponse.Data != null) {
                        for (var i = 0; i < apiResponse.Data.length; i++) {
                            var text = "span[data-valmsg-for=\"" + apiResponse.Data[i].Key + "\"]";

                            if (apiResponse.Data[i].Error != null) {
                                $(text).text(apiResponse.Data[i].Error);
                                gritter("Thông báo lỗi", apiResponse.Data[i].Error);
                            }
                            else
                                $(text).text("");
                        }
                    }
                    var ValidationSummary = document.querySelectorAll(".validation-summary-errors");
                    if (ValidationSummary != null && ValidationSummary.length > 0) {
                        for (var i = 0; i < ValidationSummary.length; i++) {
                            gritter("Thông báo lỗi", apiResponse.Message);
                            ValidationSummary[i].innerHTML = "<ul><li>" + apiResponse.Message + "</li></ul>";
                        }
                    }
                    else {
                        gritter("Thông báo lỗi", apiResponse.Message);
                        ValidationSummary.innerHTML = "<ul><li>" + apiResponse.Message + "</li></ul>";
                    }
                }
            }
        }
        if (apiResponse.TYPE != "GetPromotion") {
            CloseLoaderDeposit("myModalEdit");
        }
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoaderDeposit("myModalEdit");
    }
}

function gritter(title, text) {
    jQuery.gritter.add({
        title: title,
        text: text,
        class_name: 'growl-danger',
        //image: 'images/screen.png',
        sticky: false,
        time: ''
    });
}
function ShowHideDivDeposit_Temp() {
    var chkYes = document.getElementById("ID_DVTDeposit_Temp");
    var PriceProductInputOutput = document.getElementById("PriceProductDeposit_Temp");
    var PriceInputOutput = document.getElementById("PriceDeposit_Temp");
    var Price_QDInputOutput = document.getElementById("Price_QDDeposit_Temp");
    if (chkYes.checked)
        PriceProductInputOutput.value = PriceInputOutput.value;
    else
        PriceProductInputOutput.value = Price_QDInputOutput.value;
}
function OnchangeCheckbox(e) {
    const checkbox = e.target;
    var array = document.querySelectorAll('#tbodytmpitem input[name="item.CHON"]')
    if (array != null && array.length > 0) {
        for (var i = 0; i < array.length; i++) {
            array[i].checked = checkbox.checked;
        }
    }
}

function OnSubmit() {
    try {
        var e = document.getElementById("HINHTHUC_THUCHIEN");
        var value = e.value;
        var text = e.options[e.selectedIndex].text;
        if (confirm("Bạn muốn thực hiện tạo phiếu xuất " + text + "!")) {
            OpenLoader();
            var cartList = [];
            var array = document.querySelectorAll('#tbodytmpitem input[name="item.CHON"]')
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
                        url: "/Deposit/OnSubmitDeposit",
                        data: { cartOrder: JSON.stringify(cartList), HINHTHUC: value },
                        dataType: "json",
                        success: OnSubmitDeposit,
                        error: CloseLoader
                    });
                } else {
                    CloseLoader();
                    alert("Vui lòng chọn ít nhất 1 phiếu!");
                }
            }
            else {
                CloseLoader();
                alert("Vui lòng chọn ít nhất 1 phiếu!");
            }
        }
    }
    catch (ex) {
        alert(ex);
    }
}

function OnSubmitDeposit(data) {
    try
    {
        if (data.URL != null && data.URL != "")
        { window.location.href = data.URL; }
        else {
            if (data.Message != null && data.Message != "") { alert(data.Message); }
            location.reload();
        }
        CloseLoader();
    }
    catch
    {
        CloseLoader();
    }
   
}
function funSearchItemOrder(Controller) {
    try {
        var x = document.getElementById("Show");
        var value = x.options[x.selectedIndex].value;
        //var x1 = document.getElementById("ShowSearchValue");
        //var value1 = x1.options[x1.selectedIndex].value;
        var value1 = "";
        var SearchString = document.getElementById("SearchString").value;
        var fromdate = document.getElementById("fromdate").value;
        var todate = document.getElementById("todate").value;
        var y = document.getElementById("id_depot");
        var id_depot = y.options[y.selectedIndex].value;
        y = document.getElementById("lstID_KHUVUCSEARCH");
        var ID_KHUVUC = y.options[y.selectedIndex].value;
        location.replace("/" + Controller + "?Page=" + 1 + "&ID_DEPOT=" + id_depot + "&ID_KHUVUC=" + ID_KHUVUC + "&ShowSearchValue=" + value1 + "&SearchString=" + SearchString + "&FromDate=" + fromdate + "&ToDate=" + todate);
    }
    catch (ex) {
        alert(ex.Message);
    }
}

function Save_Map()
{
    try
    {
        OpenLoader();
        var LATITUDE = document.getElementById("LATITUDE").value;
        var LONGITUDE = document.getElementById("LONGITUDE").value;
        var ID = document.getElementById("lstdm_KhachHang").value;
        $.ajax({
            type: "POST",
            url: "/Deposit/SaveMapCustomer",
            data: "{ID:'" + ID + "',LATITUDE:'" + LATITUDE + "',LONGITUDE:'" + LONGITUDE + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSubmitSave_Map,
            error: myFunClosedProduct
        });
    }
    catch (ex) {
        CloseLoader();
        alert(ex);
    }
}

function OnSubmitSave_Map(data) {
    try
    {
        alert(data);
        CloseLoader();
    }
    catch
    {
        CloseLoader();
    }
}

function OpenMap()
{
    try
    {
        var LATITUDE = document.getElementById("LATITUDE").value;
        var LONGITUDE = document.getElementById("LONGITUDE").value;
        var ID = document.getElementById("lstdm_KhachHang").value;
        window.open(window.location.origin + "/Map?LATITUDE=" + LATITUDE + "&LONGITUDE=" + LONGITUDE + "&ID=" + ID, '_blank');
    }
    catch (ex)
    {
        alert(ex);
    }
}
