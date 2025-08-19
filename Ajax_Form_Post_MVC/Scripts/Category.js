
$(document).ready(function ()
{
    $("#SearchString").on("keyup", function (event)
    {
        if (event.keyCode == 13)
        {
            if (location != null && location.pathname != null)
            {
                var lstString = location.pathname.split("/");
                if (lstString.length > 0)
                {
                    funSearchItem(lstString[1]);
                }
              
            }
        }
    });
    $("#SearchStringSearch").on("keyup", function (event) {
        if (event.keyCode == 13) {
            document.getElementById("ButtonClick").click();
        }
    });
});

function OpenLoaderCategory() {
    try {
        var modal = document.getElementById("myModal1");
        modal.style.display = "block";
        modal.focus();
        modal.click();
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
   
}

function CloseLoaderCategory() {
    try {
        var modal = document.getElementById("myModal1");
        modal.style.display = "none";
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
}

function OnchangeCheckbox(e, NameBody) {
    try {
        const checkbox = e.target;
        var array = document.querySelectorAll('#' + NameBody +' input[type="checkbox"]')
        if (array != null && array.length > 0) {
            for (var i = 0; i < array.length; i++) {
                array[i].checked = checkbox.checked;
            }
        }
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}
function myFunction(Controller, id)
{
    if (confirm("Bạn muốn thực hiện xóa!"))
    {
        $.post("/" + Controller + "/Delete?id=" + id + "", null,
            function (data) {
                location.reload();
            });
    }
}

function show(Controller)
{
    var e = document.getElementById("Show");
    var value = e.options[e.selectedIndex].value;
    $.post("/" + "Admin/show", { "Show": value, "Controller": Controller }, function (data)
    {
        location.replace("/" + Controller + "?Page=" + 1);
    });
}
function funSearchItem(Controller)
{
    try
    {
        var x = document.getElementById("Show");
        var value = x.options[x.selectedIndex].value;
        var x1 = document.getElementById("ShowSearchValue");
        var value1 = x1.options[x1.selectedIndex].value;
        var z = document.getElementById("SearchString").value;
        location.replace("/" + Controller + "?Page=" + 1 + "&ShowSearchValue=" + value1 + "&SearchString=" + z);
    }
    catch (ex)
    {
        alert(ex.Message);
        alert(ex);
    }
}

//Endregion

//try {
//} catch (ex) {
//    alert(ex.Message);
//}


// #region Popup Delete
function myFunctionPopup(Controller, id) {
    if (confirm("Bạn muốn thực hiện xóa!")) {
        $.ajax({
            type: "POST",
            url: "/" + Controller + "/DeletePopup",
            data: "{id:'" + id + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessDelete,
            error: OnErrorLoadEdit
        });
    }
}
function OnSuccessDelete(apiResponse) {
    try
    {
        if (apiResponse.Success)
        {
            var array = $("#tbodytmpitem tr");
            if (array != null && array.length > 0)
            {
                for (var i = 0; i < array.length; i++)
                {
                    if (array[i].id == apiResponse.ID)
                    {
                        var row = document.getElementById(apiResponse.ID);
                        row.parentNode.removeChild(row);
                        break;
                    }
                }
            }
        }
        else
        { 
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else
                alert(apiResponse.Message);
        }
    }
    catch (ex)
    {
        alert(ex.Message);
        alert(ex);
    }

}
// #endregion

// #region Popup Load Data Edit
function myFunctionEdit(Controller, id)
{
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/" + Controller + "/EditPopup?id=" + id,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessLoadEdit,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}

function OnSuccessLoadEdit(apiResponse)
{
    try
    {
        CloseLoaderCategory();
        if (apiResponse.Success)
        {
            var bolChaymaskinput = false;
            var bolErrorISACTIVE = false;
            var i, j, m, n, p = 0;
            myFunOpen("myModalEdit");
            var divElem = document.getElementById('myModalEdit');
            var lst = divElem.querySelectorAll("input, select, checkbox, textarea, lable, img, tbody, radio");
            for (i = 0; i < apiResponse.Detail.length; i++) {
                if (apiResponse.Detail[i].Key == "myProductEdit") {
                    var myProduct = document.getElementById('myProduct');
                    if (myProduct != null) {
                        myProduct.innerHTML = "";
                    }
                    var myProductEdit = document.getElementById('myProductEdit');
                    if (myProductEdit != null) {
                        myProductEdit.innerHTML = apiResponse.Detail[i].Value;
                    }
                    continue;
                }
                //var lst = document.getElementById(apiResponse[i].Key);
                if (lst != null && lst.length > 0) {
                    for (j = 0; j < lst.length; j++)
                    {
                      
                        if (lst[j].name == apiResponse.Detail[i].Key || lst[j].id == apiResponse.Detail[i].Key || lst[j].name == "MaHinhEdit" || (lst[j].id == "tbodyTempItemdivPromotion_YCEdit" && lst[j].id == apiResponse.Detail[i].Key) || (lst[j].id == "tbodyTempItemdivPromotion_TangEdit" && lst[j].id == apiResponse.Detail[i].Key) || (lst[j].id == "tbodyTempItemComboEdit" && lst[j].id == apiResponse.Detail[i].Key) || (lst[j].id == "tbodyTempItemInputEdit" && lst[j].id == apiResponse.Detail[i].Key))
                        {
                            if (lst[j].type == "checkbox")
                                lst[j].checked = apiResponse.Detail[i].Value;
                            else if (lst[j].localName == "tbody")
                            {
                                lst[j].innerHTML = apiResponse.Detail[i].Value;
                                if (!bolChaymaskinput)
                                    bolChaymaskinput = true;
                            }
                            else if (lst[j].name == "MaHinhEdit") {
                                lst[j].value = "";
                            }
                            else if (lst[j].type == "radio") {
                                $('input:radio[name=\"' + lst[j].name +'\"][value=\"' + apiResponse.Detail[i].Value + '\"]').prop('checked', true);
                            }
                            else if (lst[j].localName == "img") {
                                var reader = new FileReader();

                                var imgtag = document.getElementById(lst[j].name +"Edit");
                                imgtag.title = apiResponse.Detail[i].Value;
                                const imageUrl = apiResponse.PathProduct + apiResponse.Detail[i].Value + `?v=${Date.now()}`;
                                imgtag.src = imageUrl;
                                $("#" + lst[j].name + "Edit").attr("src", imageUrl);
                            }
                            else if (lst[j].type == "select-one") {
                                var select = lst[j];
                                var option1 = select.options[0];

                                $(select).empty();
                                $(select).append(option1);
                                for (m = 0; m < apiResponse.Detail.length; m++) {
                                    if (lst[j].id == (apiResponse.Detail[m].Key + "Edit") && apiResponse.Detail[m].Value != null)
                                    {
                                        for (n = 0; n < apiResponse.Detail[m].Value.length; n++)
                                        {
                                            if (bolErrorISACTIVE == false) {
                                                try {
                                                    if (apiResponse.Detail[m].Value[n].ISACTIVE != null) {
                                                        if (apiResponse.Detail[m].Value[n].ISACTIVE == true) {
                                                            $(select).append('<option value=' + apiResponse.Detail[m].Value[n].ID + '>' + apiResponse.Detail[m].Value[n].NAME + '</option>');
                                                        }
                                                        else {
                                                            if (apiResponse.Detail[i].Value == apiResponse.Detail[m].Value[n].ID) {
                                                                $(select).append('<option value=' + apiResponse.Detail[m].Value[n].ID + '>' + apiResponse.Detail[m].Value[n].NAME + '</option>');
                                                            }
                                                        }
                                                    }
                                                    else {
                                                        $(select).append('<option value=' + apiResponse.Detail[m].Value[n].ID + '>' + apiResponse.Detail[m].Value[n].NAME + '</option>');
                                                    }
                                                }
                                                catch
                                                {
                                                    bolErrorISACTIVE = true;
                                                    if (apiResponse.Detail[i].Value == apiResponse.Detail[m].Value[n].ID) {
                                                        $(select).append('<option value=' + apiResponse.Detail[m].Value[n].ID + '>' + apiResponse.Detail[m].Value[n].NAME + '</option>');
                                                    }
                                                }    
                                            }
                                            else {
                                                $(select).append('<option value=' + apiResponse.Detail[m].Value[n].ID + '>' + apiResponse.Detail[m].Value[n].NAME + '</option>');
                                            }
                                        }
                                        lst[j].value = apiResponse.Detail[i].Value;
                                        jQuery("#" + lst[j].id).val(apiResponse.Detail[i].Value);
                                        jQuery("#" + lst[j].id).trigger("chosen:updated");
                                        $("#" + lst[j].id).val(apiResponse.Detail[i].Value).change();
                                        break;
                                    }
                                }
                            }
                            else {
                                if (lst[j].type == "date") {
                                    try {
                                        lst[j].value = apiResponse.Detail[i].Value.split(' ')[0];
                                    } catch { }
                                }
                                else if (lst[j].type == "time") {
                                    try {
                                        lst[j].value = apiResponse.Detail[i].Value;
                                    }
                                    catch { }
                                }
                                else if (lst[j].type == "datetime-local") {
                                    try {
                                        lst[j].value = apiResponse.Detail[i].Value.substring(0, 16);
                                        //lst[j].value = apiResponse.Detail[i].Value;
                                    }
                                    catch { }
                                }
                                else
                                    lst[j].value = apiResponse.Detail[i].Value;
                            }
                            continue;
                        }

                    }
                }
            }

            if (apiResponse.TYPE == "divNCCEdit") {
                var imgtagNV = document.getElementById("divNHANVIENEdit");
                var imgtag = document.getElementById("divKHACHHANGEdit");
                var imgtagCombo = document.getElementById("divNCCEdit");
                var imgtagXE = document.getElementById("divXeEdit");
                if (imgtagNV != null) {
                    imgtagNV.style.visibility = 'hidden';
                    // OR
                    imgtagNV.style.display = 'none';
                }
                if (imgtagXE != null) {
                    imgtagXE.style.visibility = 'hidden';
                    // OR
                    imgtagXE.style.display = 'none';
                }
                if (imgtag != null) {
                    imgtag.style.visibility = 'hidden';
                    // OR
                    imgtag.style.display = 'none';
                }
                if (imgtagCombo != null) {
                    imgtagCombo.style.visibility = 'visible';
                    // OR
                    imgtagCombo.style.display = 'block';
                }
            }
            else if (apiResponse.TYPE == "divKHACHHANGEdit") {
                var imgtagNV = document.getElementById("divNHANVIENEdit");
                var imgtag = document.getElementById("divKHACHHANGEdit");
                var imgtagCombo = document.getElementById("divNCCEdit");
                var imgtagXE = document.getElementById("divXeEdit");

                if (imgtagNV != null) {
                    imgtagNV.style.visibility = 'hidden';
                    // OR
                    imgtagNV.style.display = 'none';
                }
                if (imgtagXE != null) {
                    imgtagXE.style.visibility = 'hidden';
                    // OR
                    imgtagXE.style.display = 'none';
                }
                if (imgtag != null) {
                    imgtag.style.visibility = 'visible';
                    // OR
                    imgtag.style.display = 'block';
                }
                if (imgtagCombo != null) {
                    imgtagCombo.style.visibility = 'hidden';
                    // OR
                    imgtagCombo.style.display = 'none';
                }
            } else if (apiResponse.TYPE == "divNHANVIENEdit")
            {
                var imgtagNV = document.getElementById("divNHANVIENEdit");
                var imgtag = document.getElementById("divKHACHHANGEdit");
                var imgtagCombo = document.getElementById("divNCCEdit");
                var imgtagXE = document.getElementById("divXeEdit");
                if (imgtagNV != null) {
                    imgtagNV.style.visibility = 'visible';
                    // OR
                    imgtagNV.style.display = 'block';
                }
                if (imgtagXE != null) {
                    imgtagXE.style.visibility = 'hidden';
                    // OR
                    imgtagXE.style.display = 'none';
                }
                if (imgtag != null) {
                    imgtag.style.visibility = 'hidden';
                    // OR
                    imgtag.style.display = 'none';
                }
                if (imgtagCombo != null) {
                    imgtagCombo.style.visibility = 'hidden';
                    // OR
                    imgtagCombo.style.display = 'none';
                }
            }
            else if (apiResponse.TYPE == "divXeEdit") {
                var imgtagNV = document.getElementById("divNHANVIENEdit");
                var imgtag = document.getElementById("divKHACHHANGEdit");
                var imgtagCombo = document.getElementById("divNCCEdit");
                var imgtagXE = document.getElementById("divXeEdit");
                if (imgtagXE != null) {
                    imgtagXE.style.visibility = 'visible';
                    // OR
                    imgtagXE.style.display = 'block';
                }
                if (imgtagNV != null) {
                    imgtagNV.style.visibility = 'hidden';
                    // OR
                    imgtagNV.style.display = 'none';
                }
                if (imgtag != null) {
                    imgtag.style.visibility = 'hidden';
                    // OR
                    imgtag.style.display = 'none';
                }
                if (imgtagCombo != null) {
                    imgtagCombo.style.visibility = 'hidden';
                    // OR
                    imgtagCombo.style.display = 'none';
                }
            }

            if (bolChaymaskinput) {
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
        else
        {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else
                alert(apiResponse.Message);
        }
    }
    catch (ex)
    {
        alert(ex.Message);
        alert(ex);
    }
    
}
// #endregion

// #region Popup Load Data Create
function myFunctionCreate(Controller) {
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/" + Controller + "/CreatePopup",
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessLoadCreate,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
}

function myFunctionCreateTomorrow(Controller, intHINHTHUC) {
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/" + Controller + "/CreatePopup",
            data: { HINHTHUC: intHINHTHUC },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessLoadCreate,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }
}

// #region Popup Load Data Create
function myFunctionCreateTimekeeping(Controller, date, ID_TAIKHOAN) {
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "POST",
            url: "/" + Controller + "/CreatePopupDate",
            data: "{Date:'" + date + "',ID_TAIKHOAN:'" + ID_TAIKHOAN +"'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessLoadCreate,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }
}

// #region Popup Load Data Create
function myFunctionCreateInputOutput(Controller, ID_LOAIPHIEU) {
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/" + Controller + "/CreatePopup?ID_LOAIPHIEU=" + ID_LOAIPHIEU,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessLoadCreate_LOAIPHIEU,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}

function myFunctionCreateOrder_Provider(Controller, ID) {
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/" + Controller + "/CreatePopupNCC?ID=" + ID,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessLoadCreate_LOAIPHIEU,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }

}


function myFunctionCreateReceiptDelivery(Controller, ID, ID_LOAIPHIEU, ID_KHACHAHANG, CHUNGTUKEMTHEO) {
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/" + Controller + "/CreatePopup?ID=" + ID + "&ID_LOAIPHIEU=" + ID_LOAIPHIEU + "&ID_KHACHAHANG=" + ID_KHACHAHANG + "&CHUNGTUKEMTHEO=" + CHUNGTUKEMTHEO ,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessLoadCreate_LOAIPHIEU,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }

}

function OnSuccessLoadCreate_LOAIPHIEU(data)
{
    try {
        OnSuccessLoadCreate(data);
        var xKho = document.querySelectorAll('[id=lstdm_Kho]');
        if (xKho != null && xKho.length > 0) {
            for (var i = 0; i < xKho.length; i++) {
                if (data.NAME != "myModalAdd") {
                    document.getElementById(xKho[i].id + "_chosen").style.pointerEvents = "none";
                    //xKho[i].disabled = false;
                }
            }
        }
        if (data.TYPE == "divNCCAdd") {
            
            var xMP = document.querySelectorAll('[id=divNHANVIENAdd]');
            if (xMP != null && xMP.length > 0)
            {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'hidden';
                    xMP[i].style.display = 'none';
                }
            }
            xMP = document.querySelectorAll('[id=divKHACHHANGAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'hidden';
                    xMP[i].style.display = 'none';
                }
            }
            xMP = document.querySelectorAll('[id=divNCCAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'visible';
                    xMP[i].style.display = 'block';
                }
            }

            xMP = document.querySelectorAll('[id=divXEAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'hidden';
                    xMP[i].style.display = 'none';
                }
            }
        }
        else if (data.TYPE == "divKHACHHANGAdd") {
            var xMP = document.querySelectorAll('[id=divNHANVIENAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'hidden';
                    xMP[i].style.display = 'none';
                }
            }
            xMP = document.querySelectorAll('[id=divKHACHHANGAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'visible';
                    xMP[i].style.display = 'block';
                }
            }

            xMP = document.querySelectorAll('[id=divNCCAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'hidden';
                    xMP[i].style.display = 'none';
                }
            }

            xMP = document.querySelectorAll('[id=divXEAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'hidden';
                    xMP[i].style.display = 'none';
                }
            }
        }
        else if (data.TYPE == "divNHANVIENAdd") {
            var xMP = document.querySelectorAll('[id=divNHANVIENAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'visible';
                    xMP[i].style.display = 'block';
                }
            }
            xMP = document.querySelectorAll('[id=divKHACHHANGAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'hidden';
                    xMP[i].style.display = 'none';
                }
            }

            xMP = document.querySelectorAll('[id=divNCCAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'hidden';
                    xMP[i].style.display = 'none';
                }
            }

            xMP = document.querySelectorAll('[id=divXEAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'hidden';
                    xMP[i].style.display = 'none';
                }
            }
        }
        else if (data.TYPE == "divXEAdd") {
            var xMP = document.querySelectorAll('[id=divXEAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'visible';
                    xMP[i].style.display = 'block';
                }
            }
            xMP = document.querySelectorAll('[id=divKHACHHANGAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'hidden';
                    xMP[i].style.display = 'none';
                }
            }

            xMP = document.querySelectorAll('[id=divNCCAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'hidden';
                    xMP[i].style.display = 'none';
                }
            }

            xMP = document.querySelectorAll('[id=divNHANVIENAdd]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'hidden';
                    xMP[i].style.display = 'none';
                }
            }
        }
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}

function OnSuccessLoadCreate(apiResponse)
{
    try
    {
        if (apiResponse.Success)
        {
            var bolError = false;
            var bolErrorISACTIVE = false;
            var i, j, m, n = 0;
            if (apiResponse.NAME == null || apiResponse.NAME == "") {
                apiResponse.NAME = "myModalAdd";
            }
            myFunOpen(apiResponse.NAME);
            var divElem = document.getElementById(apiResponse.NAME);
            var lst = divElem.querySelectorAll("input, select, checkbox, textarea, lable, img, radio, tbody");
            for (i = 0; i < apiResponse.Detail.length; i++) {
                if (apiResponse.Detail[i].Key == "lblName") {
                    var xMP = document.querySelectorAll('[id=' + apiResponse.Detail[i].Key +']');
                    if (xMP != null && xMP.length > 0) {
                        for (var k = 0; k < xMP.length; k++) {
                            xMP[k].innerHTML = apiResponse.Detail[i].Value;
                        }
                    }
                    continue;
                }
                if (apiResponse.Detail[i].Key == "myProduct") {

                    var myProductEdit = document.getElementById('myProductEdit');
                    if (myProductEdit != null) {
                        myProductEdit.innerHTML = "";
                    }
                    var myProduct = document.getElementById('myProduct');
                    if (myProduct != null) {
                        myProduct.innerHTML = apiResponse.Detail[i].Value;
                    }
                    continue;
                }
                if (lst != null && lst.length > 0)
                {
                    for (j = 0; j < lst.length; j++) {
                        
                        if (lst[j].name == apiResponse.Detail[i].Key || lst[j].id == apiResponse.Detail[i].Key || lst[j].id == "tbodyTempItemInput")
                        {
                            if (lst[j].type == "checkbox")
                                lst[j].checked = apiResponse.Detail[i].Value;
                            else if (lst[j].localName == "img") {
                                var reader = new FileReader();
                                var imgtag = document.getElementById(lst[j].name + "Add");
                                imgtag.title = "";
                                imgtag.src = "";
                            }
                            else if (lst[j].localName == "tbody") {
                                lst[j].innerHTML = apiResponse.Detail[i].Value;

                            }
                            else if (lst[j].type == "radio") {

                            }
                            else if (lst[j].type == "select-one") {
                                if (apiResponse.NAME != "myModalAdd")
                                {
                                    if (lst[j].id == "lstdm_Kho" || lst[j].id == "lstdm_NhaCungCap" || lst[j].id == "lstdm_KhachHang")
                                    {
                                        var xMP_chosen = document.querySelectorAll('[id=' + lst[j].id + "_chosen" + ']');
                                        if (xMP_chosen != null && xMP_chosen.length > 0) {
                                            for (var k = 0; k < xMP_chosen.length; k++)
                                            {
                                                xMP_chosen[k].style.pointerEvents = "none";
                                            }
                                        }
                                    }
                                }
                                var select = lst[j];
                                var option1 = select.options[0];

                                var valuetext = null;
                                $(select).empty();
                                $(select).append(option1);
                                for (m = 0; m < apiResponse.Detail.length; m++) {
                                    if (lst[j].id == apiResponse.Detail[m].Key && apiResponse.Detail[m].Value != null) {
                                        for (n = 0; n < apiResponse.Detail[m].Value.length; n++) {
                                            if (bolErrorISACTIVE == false) {
                                                try {
                                                    if (apiResponse.Detail[m].Value[n].ISACTIVE != null) {
                                                        if (apiResponse.Detail[m].Value[n].ISACTIVE == true) {
                                                            $(select).append('<option value=' + apiResponse.Detail[m].Value[n].ID + '>' + apiResponse.Detail[m].Value[n].NAME + '</option>');
                                                        }
                                                    }
                                                    else {
                                                        $(select).append('<option value=' + apiResponse.Detail[m].Value[n].ID + '>' + apiResponse.Detail[m].Value[n].NAME + '</option>');
                                                    }
                                                }
                                                catch
                                                {
                                                    $(select).append('<option value=' + apiResponse.Detail[m].Value[n].ID + '>' + apiResponse.Detail[m].Value[n].NAME + '</option>');
                                                    bolErrorISACTIVE = true;
                                                }
                                            }
                                            else {
                                                $(select).append('<option value=' + apiResponse.Detail[m].Value[n].ID + '>' + apiResponse.Detail[m].Value[n].NAME + '</option>');
                                            }


                                            if (bolError == false) {
                                                try {
                                                    if (apiResponse.Detail[m].Value[n].ISDEFAULT == true) {
                                                        valuetext = apiResponse.Detail[m].Value[n].ID;
                                                    }
                                                }
                                                catch
                                                {
                                                    bolError = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                lst[j].value = valuetext;
                                jQuery(select).val(valuetext);
                                jQuery(select).trigger("chosen:updated");
                                $(select).val(valuetext).change();
                                //jQuery("#" + lst[j].id).val(valuetext);
                                //jQuery("#" + lst[j].id).trigger("chosen:updated");
                                //$("#" + lst[j].id).val(valuetext).change();
                            }
                            else {
                                if (lst[j].type == "date") {
                                    try {
                                        lst[j].value = apiResponse.Detail[i].Value.split(' ')[0];
                                    } catch { }
                                }
                                else
                                    lst[j].value = apiResponse.Detail[i].Value;
                            }
                            continue;
                        }

                    }
                }
            }
        }
        else
        {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else
                alert(apiResponse.Message);
        }
        CloseLoaderCategory();
    }
    catch (ex)
    {
        CloseLoaderCategory();
        alert(ex.Message);
        alert(ex);
    }

}
// #endregion
function OnErrorLoadEdit()
{
    alert("Lỗi: vui lòng liên hệ nhà cung cấp! Xin cảm ơn. Category");
    CloseLoaderCategory();
}

// #region Popup Create Succes
function OnBegin() {
    OpenLoaderCategory();
}

function OnEnd() {
    CloseLoaderCategory();
}

function OnSuccessCreate(apiResponse)
{
    try
    {
        if (apiResponse.Success)
        {
            var x = document.getElementsByName("ID");
            if (x != null && x.length > 0)
            {
                for (var i = 0; i < x.length; i++)
                {
                    x[i].value = apiResponse.NewID;
                }
            }
            var xMP = document.getElementsByName("MAPHIEU");
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].value = apiResponse.MAPHIEU;
                }
            }
            if (apiResponse.MAPHIEU == "" || apiResponse.MAPHIEU == null)
            {
                location.reload();
            }
            var xSP = document.getElementsByName("SOPHIEU");
            if (xSP != null && xSP.length > 0) {
                for (var i = 0; i < xSP.length; i++) {
                    xSP[i].value = apiResponse.SOPHIEU;
                }
            }

            var xBody = document.getElementById("tbodyTempItemInput");
            if (xBody != null) {
               xBody.innerHTML = "";
            }
            if (confirm("Tạo thành công! Bạn có muốn thực thêm mới tiếp tục!"))
            {
                myFunClosed("");
            }
            else
            {
                myFunClosed("myModalAdd");
            }
            var array = $("#tbodytmpitem tr");
            if (array != null && array.length > 0)
            {
                for (var i = 0; i < array.length; i++) {
                    var newcloneNode = array[0].cloneNode(true);
                    var lst = newcloneNode.querySelectorAll("td");
                    if (lst != null && lst.length > 0 && apiResponse.Detail != null) {
                        newcloneNode.id = apiResponse.ID;
                        for (var j = 0; j < lst.length; j++) {
                            for (var m = 0; m < apiResponse.Detail.length; m++) {
                                var lstNode = lst[j].querySelectorAll("input, select, checkbox, textarea");
                                if (lstNode != null && lstNode.length > 0) {
                                    if (lst[j].id == apiResponse.Detail[m].Key) {
                                        if (lstNode[0].type == "checkbox")
                                            lstNode[0].checked = apiResponse.Detail[m].Value;
                                        else if (lstNode[0].type == "label")
                                            lstNode[0].innerHTML = apiResponse.Detail[m].Value;
                                        else
                                            lstNode[0].value = apiResponse.Detail[m].Value;
                                    }
                                }
                                else {
                                    if (lst[j].id == apiResponse.Detail[m].Key) {
                                        if (lst[j].id == "PICTURE") {
                                            lst[j].innerHTML = '<div class="thmb-prev"><a href=\"' + apiResponse.PathProduct + apiResponse.Detail[m].Value + '" data-rel="prettyPhoto" rel="prettyPhoto"><img src="' + apiResponse.PathProduct + apiResponse.Detail[m].Value + '" class="img-responsive" alt=""></a></div>';
                                        }
                                        else
                                            lst[j].innerHTML = apiResponse.Detail[m].Value;
                                    }
                                }
                            }
                        }
                        lst[lst.length - 1].innerHTML = lst[lst.length - 1].innerHTML.replaceAll(",'" + array[0].id + "')", ",'" + apiResponse.ID + "')")
                    }

                    // Inject it into the DOM
                    array[0].before(newcloneNode);
                    break;
                }
            }
            else
            {
                location.reload();
            }
        }
        else {
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
                    var divElem = document.getElementById("myModalAdd");
                    x = divElem.querySelectorAll(".validation-summary-errors");
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
                    var divElem = document.getElementById("myModalAdd");
                    var ValidationSummary = divElem.querySelectorAll(".validation-summary-errors");
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
        CloseLoaderCategory();
    }
    catch (ex)
    {
        alert(ex.Message);
        alert(ex);
        CloseLoaderCategory();
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
// #endregion

// #region Popup Edit Succes
function OnSuccessEdit(apiResponse)
{
    try
    {
        if (apiResponse.Success)
        {
            var i, j, m = 0;
            var array = $("#tbodytmpitem tr[id=\"" + apiResponse.ID + "\"] td");
            if (array != null && array.length > 0)
            {
                for (m = 0; m < array.length; m++)
                {
                    var lst = array[m].querySelectorAll("input, select, checkbox, textarea");
                    if (lst != null && lst.length > 0)
                    {
                        for (i = 0; i < apiResponse.Detail.length; i++)
                        {

                            for (j = 0; j < lst.length; j++) {
                                if (array[m].id == apiResponse.Detail[i].Key)
                                {
                                    if (lst[j].type == "checkbox")
                                        lst[j].checked = apiResponse.Detail[i].Value;
                                    else if (lst[j].type == "label")
                                        lst[j].innerHTML = apiResponse.Detail[i].Value;
                                    else
                                        lst[j].value = apiResponse.Detail[i].Value;

                                    continue;
                                }

                            }
                        }
                    }
                    else
                    {
                        for (i = 0; i < apiResponse.Detail.length; i++)
                        {
                            if (array[m].id == apiResponse.Detail[i].Key)
                            {
                                if (array[m].id == "PICTURE")
                                {
                                    const imageUrl = apiResponse.PathProduct + apiResponse.Detail[i].Value + `?v=${Date.now()}`;
                                    array[m].innerHTML = '<div class="thmb-prev"><a href="' + imageUrl + '" data-rel="prettyPhoto" rel="prettyPhoto"><img src="' + imageUrl + '" class="img-responsive" alt=""></a></div>';
                                }
                                else
                                   array[m].innerHTML = apiResponse.Detail[i].Value;
                                continue;
                            }
                        }

                    }
                }
            }
            myFunClosed("myModalEdit");
        }
        else
        {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else
            {
                if (apiResponse.Data != null)
                {
                    for (var i = 0; i < apiResponse.Data.length; i++) {
                        var text = "span[data-valmsg-for=\"" + apiResponse.Data[i].Key + "\"]";

                        if (apiResponse.Data[i].Error != null) {
                            gritter("Thông báo lỗi", apiResponse.Data[i].Error);
                            $(text).text(apiResponse.Data[i].Error);
                        }
                        else
                            $(text).text("");
                    }
                }
                var divElem = document.getElementById("myModalEdit");
                var ValidationSummary = divElem.querySelectorAll(".validation-summary-errors");
                if (ValidationSummary != null && ValidationSummary.length > 0) {
                    for (var i = 0; i < ValidationSummary.length; i++) {
                        gritter("Thông báo lỗi", apiResponse.Message);
                        ValidationSummary[i].innerHTML = "<ul><li>" + apiResponse.Message + "</li></ul>";
                    }
                }
                else { 
                    ValidationSummary.innerHTML = "<ul><li>" + apiResponse.Message + "</li></ul>";
                    gritter("Thông báo lỗi", apiResponse.Message);
                }
            }
        }
        CloseLoaderCategory();
    }
    catch (ex)
    {
        alert(ex.Message);
        alert(ex);
        CloseLoaderCategory();
    }
}
// #endregion

function myFunOpenSearch(myModal, Controller, HinhThucTimKiem, ValueField, TextField)
{
    try {
        var divElem = document.getElementById(myModal);
        var lst = divElem.querySelectorAll("select");
        var ID_KHO = "";
        if (myModal != "collapseOneDebt" && lst != null && lst.length > 0) {
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
        OpenLoaderCategory();
        $.ajax({
            type: "POST",
            url: "/Search/LoadSearch",
            data: "{MyModal:'" + myModal + "',ClassName:'" + Controller + "',HinhThucTimKiem:" + HinhThucTimKiem + ",ValueField:'" + ValueField + "',TextField:'" + TextField + "',ID_KHO:'" + ID_KHO + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessSearch
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
   
}

function OnSuccessSearch(Data)
{
    try {
        CloseLoaderCategory();
        var trSearch = document.getElementById("trSearch");
        trSearch.innerHTML = Data.TrField;
        var tbodySearch = document.getElementById("tbodySearch");
        tbodySearch.innerHTML = Data.BodyField;
        var lblNameSearch = document.getElementById("lblNameSearch");
        lblNameSearch.innerHTML = Data.TitleSearch;
        var ClassNameSearch = document.getElementById("ClassNameSearch");
        ClassNameSearch.value = Data.ClassName;
        var ValueFieldSearch = document.getElementById("ValueFieldSearch");
        ValueFieldSearch.value = Data.ValueField;
        var TextFieldSearch = document.getElementById("TextFieldSearch");
        TextFieldSearch.value = Data.TextField;
        var MyModalSearch = document.getElementById("MyModalSearch");
        MyModalSearch.value = Data.MyModal;
        var ID_KHOSEARCH = document.getElementById("ID_KHOSEARCH");
        ID_KHOSEARCH.value = Data.ID_KHO;
        var HinhThucTimKiemSearch = document.getElementById("HinhThucTimKiemSearch");
        HinhThucTimKiemSearch.value = Data.HinhThucTimKiem;
        var divElem = document.getElementById('myModalSearch');
        var lst = divElem.querySelectorAll("select");
        if (lst != null && lst.length > 0) {
            for (var j = 0; j < lst.length; j++) { 
                if (lst[j].id == "ShowSearchValueSearch") { 
                    var select = lst[j];
                    $(select).empty();
                    for (var i = 0; i < Data.listSearch.length; i++) {
                        $(select).append('<option value=' + Data.listSearch[i].Item1 + '>' + Data.listSearch[i].Item2 + '</option>');
                    }
                    lst[j].value = Data.ShowSearchValue;
                    jQuery("#" + lst[j].id).val(Data.ShowSearchValue);
                    jQuery("#" + lst[j].id).trigger("chosen:updated");
                    $("#" + lst[j].id).val(Data.ShowSearchValue).change();
                }
            }
        }
        if (Data.ID_KHUVUC != null && Data.ID_KHUVUC != "") {

            var xMP = document.querySelectorAll('[id=ID_KHUVUCSEARCH]');
            if (xMP != null && xMP.length > 0) {
                for (var i = 0; i < xMP.length; i++) {
                    xMP[i].style.visibility = 'visible';
                    xMP[i].style.display = 'block';
                }
            }
            else {
                var xMP = document.querySelectorAll('[id=ID_KHUVUCSEARCH]');
                if (xMP != null && xMP.length > 0) {
                    for (var i = 0; i < xMP.length; i++) {
                        xMP[i].style.visibility = 'hidden';
                        xMP[i].style.display = 'none';
                    }
                }
            }
        }
        myFunOpen("myModalSearch");
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }
}

function myFunSuccess(ID)
{
    /*alert(ID)*/
    try {
        var ValueFieldSearch = document.getElementById("ValueFieldSearch");
        var MyModalSearch = document.getElementById("MyModalSearch");

        var divElem = document.getElementById(MyModalSearch.value);
        var lst = divElem.querySelectorAll("input, select");
        if (lst != null && lst.length > 0) {
            for (var j = 0; j < lst.length; j++) {
                if (lst[j].id == ValueFieldSearch.value) {
                    lst[j].value = ID;
                    //for (let i = 0, len = lst[j].length; i < len; i++) {
                    //    if (lst[j][i].value == ID) {
                    //        lst[j].options[i].selected = true;
                    //    }  
                    //}
                    jQuery("#" + lst[j].id).val(ID);
                    jQuery("#" + lst[j].id).trigger("chosen:updated");
                    $("#" + lst[j].id).val(ID).change();
                    $(lst[j]).change();
                    myFunClosed('myModalSearch');
                    break;
                }
            }
        }
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
   
}

// Get the modal
var modalAdd = document.getElementById("myModalAdd");

// Get the modal
var modalEdit = document.getElementById("myModalEdit");

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    if (event.target == modalAdd) {
        modalAdd.style.display = "none";
    }
    if (event.target == modalEdit) {
        modalEdit.style.display = "none";
    }
}
function myFunSuccessCombo(ID)
{
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/Product_Combo/LoadProduct?ID=" + ID,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessCombo,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}

function OnSuccessCombo(apiResponse)
{
    try {
        if (apiResponse.Success)
        {
            var radiovalue = "";
            var nf = Intl.NumberFormat('vi-VN');
            for (var m = 0; m < apiResponse.Detail.length; m++) {
                if (apiResponse.Detail[m].Key == "NAME") {
                    document.getElementById("lblNameCombo").innerHTML = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID") {
                    document.getElementById("idProductCombo").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_DVT") {
                    document.getElementById("ID_DVTCombo").value = apiResponse.Detail[m].Value;
                    radiovalue = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_DVT_QD") {
                    if (apiResponse.Detail[m].Value != null && apiResponse.Detail[m].Value != "") {
                        document.getElementById("ID_DVT_QDCombo").value = apiResponse.Detail[m].Value;
                        document.getElementById("ID_DVT_QDCombo").style.display = "";
                    }
                    else {
                        document.getElementById("ID_DVT_QDCombo").style.display = "none";
                        document.getElementById("ID_DVT_QDCombo").value = "";
                    }
                    continue;
                }
                if (apiResponse.Detail[m].Key == "NAME_DVT") {
                    document.getElementById("lblID_DVTCombo").innerHTML = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "NAME_DVT_QD") {
                    if (apiResponse.Detail[m].Value != null && apiResponse.Detail[m].Value != "") {
                        document.getElementById("ID_DVT_QDCombo").style.display = "";
                        document.getElementById("lblID_DVT_QDCombo").innerHTML = apiResponse.Detail[m].Value;
                    }
                    else {
                        document.getElementById("ID_DVT_QDCombo").style.display = "none";
                        document.getElementById("lblID_DVT_QDCombo").innerHTML = "";
                    }
                       
                    continue;
                }
                if (apiResponse.Detail[m].Key == "GIA") {
                    document.getElementById("PriceProduct").value = apiResponse.Detail[m].Value;
                    document.getElementById("PriceProductCombo").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "GIA_QD") {
                    document.getElementById("PriceProduct_QD").value = apiResponse.Detail[m].Value;
                    continue;
                }
            }
            $('input:radio[name="ID_DVT"][value=\"' + radiovalue + '\"]').prop('checked', true);
            document.getElementById("QtyProductCombo").value = 1;
            myFunOpen("myModalCombo");
            ShowHideDiv();
        }
        else {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else
                alert(apiResponse.Message);
        }
        CloseLoaderCategory();
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoaderCategory();
    }

}

function OnSuccessProductCombo(apiResponse) {
    try {
        if (apiResponse.Success)
        {
            try {
                var tbodyTempItemCombo = document.getElementById('tbodyTempItemCombo');
                tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
                LockKho("lstdm_Kho", apiResponse.ProductCombo);
            } catch { }

            try {
                var tbodyTempItemCombo = document.getElementById('tbodyTempItemComboEdit');
                tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
                LockKho("lstdm_KhoEdit", apiResponse.ProductCombo);
            } catch { }
            myFunClosed("myModalCombo");
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

function myFunOpen(myModal) {
    try {
        var modal = document.getElementById(myModal);
        modal.style.display = "block";
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    

}
function myFunClosed(myModal)
{
    try
    {
        if (myModal != "") {
            var modal = document.getElementById(myModal);
            modal.style.display = "none";
        }
       
        var divElem = document.getElementById("myModalEdit");
        var lst = divElem.querySelectorAll("span");
        if (lst != null) {
            for (var i = 0; i < lst.length; i++) {
                if (lst[i].className == "field-validation-valid text-danger") {
                    lst[i].innerHTML = "";
                }
            }
        }
    }
    catch
    { 

    }

    try {
        var divElemAdd = document.getElementById("myModalAdd");
        var lstAdd = divElemAdd.querySelectorAll("span");
        if (lstAdd != null) {
            for (var i = 0; i < lstAdd.length; i++) {
                if (lstAdd[i].className == "field-validation-valid text-danger") {
                    lstAdd[i].innerHTML = "";
                }
            }
        }
    }
    catch {

    }

    try
    { 
         var x = document.querySelectorAll(".validation-summary-errors");
        if (x != null && x.length > 0) {
            for (var i = 0; i < x.length; i++) {
                x[i].innerHTML = "";
            }
        }
        else
                x.innerHTML = "";
        }
    catch
    {

    }
}
   
function ShowHideDiv() {
    try {
        var chkYes = document.getElementById("ID_DVTCombo");
        var PriceProductCombo = document.getElementById("PriceProductCombo");
        var PriceProduct = document.getElementById("PriceProduct");
        var PriceProduct_QD = document.getElementById("PriceProduct_QD");
        if (chkYes.checked)
            PriceProductCombo.value = PriceProduct.value;
        else
            PriceProductCombo.value = PriceProduct_QD.value;
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
  
}

// #region Popup Delete
function myFunctionDeleteProdcutCombo(Controller, id, id_dvt) {
    if (confirm("Bạn muốn thực hiện xóa!")) {
        $.ajax({
            type: "POST",
            url: "/" + Controller + "/DeleteProductCombo",
            data: "{ID_HANGHOA:'" + id + "', ID_DVT:'" + id_dvt + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessProductCombo
        });
    }
}



function ShowHideDivInputOutput() {
    try {
        var chkYes = document.getElementById("ID_DVTInputOutput");
        var PriceProductInputOutput = document.getElementById("PriceProductInputOutput");
        var PriceInputOutput = document.getElementById("PriceInputOutput");
        var Price_QDInputOutput = document.getElementById("Price_QDInputOutput");
        if (chkYes.checked)
            PriceProductInputOutput.value = PriceInputOutput.value;
        else
            PriceProductInputOutput.value = Price_QDInputOutput.value;
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
   
}

function myFunSuccessInputOutput(ID, Type, ID_KHO) {
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/Product/LoadProductKho?ID=" + ID + "&Type=" + Type + "&ID_KHO=" + ID_KHO,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessInputOutput,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
  
}

function OnSuccessInputOutput(apiResponse) {
    try {
        if (apiResponse.Success) {
            var radiovalue = "";
            var nf = Intl.NumberFormat('vi-VN');
            for (var m = 0; m < apiResponse.Detail.length; m++) {
                if (apiResponse.Detail[m].Key == "NAME") {
                    document.getElementById("lblNameInputOutput").innerHTML = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_HANGHOAKHO") {
                    document.getElementById("idProductKhoInputOutput").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_HANGHOA") {
                    document.getElementById("idProductInputOutput").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_KHO") {
                    document.getElementById("idDepotInputOutput").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_DVT") {
                    document.getElementById("ID_DVTInputOutput").value = apiResponse.Detail[m].Value;
                    radiovalue = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_DVT_QD") {
                    if (apiResponse.Detail[m].Value != null && apiResponse.Detail[m].Value != "") {
                        document.getElementById("ID_DVT_QDInputOutput").value = apiResponse.Detail[m].Value;
                        document.getElementById("ID_DVT_QDInputOutput").style.display = "";
                    }
                    else {
                        document.getElementById("ID_DVT_QDInputOutput").style.display = "none";
                        document.getElementById("ID_DVT_QDInputOutput").value = "";
                    }
                    continue;
                }
                if (apiResponse.Detail[m].Key == "NAME_DVT") {
                    document.getElementById("lblID_DVTInputOutput").innerHTML = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "NAME_DVT_QD") {
                    if (apiResponse.Detail[m].Value != null && apiResponse.Detail[m].Value != "") {
                        document.getElementById("ID_DVT_QDInputOutput").style.display = "";
                        document.getElementById("lblID_DVT_QDInputOutput").innerHTML = apiResponse.Detail[m].Value;
                    }
                    else {
                        document.getElementById("ID_DVT_QDInputOutput").style.display = "none";
                        document.getElementById("lblID_DVT_QDInputOutput").innerHTML = "";
                    }

                    continue;
                }
                if (apiResponse.Detail[m].Key == "GIA") {
                    document.getElementById("PriceInputOutput").value = apiResponse.Detail[m].Value;
                    document.getElementById("PriceProductInputOutput").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "GIA_QD") {
                    document.getElementById("Price_QDInputOutput").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_THUESUAT") {
                    document.getElementById("lstdm_ThueSuatSearch").value = apiResponse.Detail[m].Value;
                    jQuery("#lstdm_ThueSuatSearch").val(apiResponse.Detail[m].Value);
                    jQuery("#lstdm_ThueSuatSearch").trigger("chosen:updated");
                    //$("#lstdm_ThueSuatSearch").val(apiResponse.Detail[m].Value).change();
                    continue;
                }
                if (apiResponse.Detail[m].Key == "THUESUAT") {
                    document.getElementById("THUESUATProductInputOutput").value = apiResponse.Detail[m].Value;
                    continue;
                }
            }
            document.getElementById("DiscountProductInputOutput").value = 0;
            document.getElementById("PriceDiscountProductInputOutput").value = 0;
            $('input:radio[name="ID_DVT"][value=\"' + radiovalue + '\"]').prop('checked', true);
            document.getElementById("QtyProductInputOutput").value = 1;
            updateAddProduct(document.getElementById("DiscountProductInputOutput"));
            myFunOpen("myModalInputOutput");
            ShowHideDiv();

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

        CloseLoaderCategory();
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoaderCategory();
    }
}

function updateAddProduct(elem) {
    try {
        var ID_HANGHOAKHO = document.getElementById("idProductKhoInputOutput").value;
        var ID_HANGHOA = document.getElementById("idProductInputOutput").value;
        var ID_DVT = document.getElementById("ID_DVTInputOutput").value;
        var SOLUONG = document.getElementById("QtyProductInputOutput").value;
        var DONGIA = document.getElementById("PriceProductInputOutput").value;
        var CHIETKHAU = document.getElementById("DiscountProductInputOutput").value;
        var TONGTIENGIAMGIA = document.getElementById("PriceDiscountProductInputOutput").value;
        var THANHTIEN = document.getElementById("IntoMoneyProductInputOutput").value;
        var ID_THUESUAT = document.getElementById("lstdm_ThueSuatSearch").value;
        var TONGTIENVAT = document.getElementById("TaxMoneyProductInputOutput").value;
        var TONGCONG = document.getElementById("TotalProductInputOutput").value;
        var THUESUAT = document.getElementById("THUESUATProductInputOutput").value;
        var ID_KHO = document.getElementById("idDepotInputOutput").value;
        $.post("/Product/UpdateAddProduct", { "ID_KHO": ID_KHO, "TYPE": elem.name, "ID_HANGHOA": ID_HANGHOA, "ID_HANGHOAKHO": ID_HANGHOAKHO, "ID_DVT": ID_DVT, "SOLUONG": SOLUONG, "DONGIA": DONGIA, "CHIETKHAU": CHIETKHAU, "TONGTIENGIAMGIA": TONGTIENGIAMGIA, "THANHTIEN": THANHTIEN, "THUESUAT": THUESUAT, "ID_THUESUAT": ID_THUESUAT, "TONGTIENVAT": TONGTIENVAT, "TONGCONG": TONGCONG }, function (data) {
            document.getElementById("idProductKhoInputOutput").value = data.Detail.ID_HANGHOAKHO;
            document.getElementById("idProductInputOutput").value = data.Detail.ID_HANGHOA;
            document.getElementById("ID_DVTInputOutput").value = data.Detail.ID_DVT;
            document.getElementById("QtyProductInputOutput").value = data.Detail.SOLUONG;
            document.getElementById("PriceProductInputOutput").value = data.Detail.DONGIA;
            document.getElementById("DiscountProductInputOutput").value = data.Detail.CHIETKHAU;
            document.getElementById("PriceDiscountProductInputOutput").value = data.Detail.TONGTIENGIAMGIA;
            document.getElementById("IntoMoneyProductInputOutput").value = data.Detail.THANHTIEN;
            document.getElementById("lstdm_ThueSuatSearch").value = data.Detail.ID_THUESUAT;
            document.getElementById("TaxMoneyProductInputOutput").value = data.Detail.TONGTIENVAT;
            document.getElementById("TotalProductInputOutput").value = data.Detail.TONGCONG;
            document.getElementById("THUESUATProductInputOutput").value = data.Detail.THUESUAT;
            document.getElementById("idDepotInputOutput").value = data.Detail.ID_KHO;
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }
    
}
function OnSuccessProductInputOutput(apiResponse) {
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
                $(ele).removeAttr("onkeyup");
                let clone = $(ele).clone(false)
                clone.removeAttr("onkeyup"); // Xoá thuộc tính gây sự kiện
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
            myFunClosed("myModalInputOutput");
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
function validatetext() {
    try {
        $("input.form-control.mask").each((i, ele) => {
            let ele1 = $(ele)
            if (ele.type == "text") {
                $(ele1).hide()
            }
            else {
                $(ele1).show()
            }
        })
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
   
}

function updateInputOutput(ID, elem) {
    try {
        $.post("/Product/UpdateProductInputOutput", { "ID": ID, "TYPE": elem.id, "VALUE": elem.value }, function (apiResponse) {
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
                        $(ele).removeAttr("onkeyup");
                        let clone = $(ele).clone(false)
                        clone.removeAttr("onkeyup"); // Xoá thuộc tính gây sự kiện
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
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 

    
}

function myFunctionDeleteProdcutInputOutput(Controller, id) {
    if (confirm("Bạn muốn thực hiện xóa!")) {
        $.ajax({
            type: "POST",
            url: "/" + Controller + "/DeleteProductInputOutput",
            data: "{ID:'" + id + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessProductInputOutput,
            error: OnErrorLoadEdit
        });
    }
}



function myFunSuccessPromotion_YC(ID, Type, ID_KHO) {
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/Product/LoadProduct?ID=" + ID + "&Type=" + Type + "&ID_KHO=" + ID_KHO,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessPromotion_YC,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}

function myFunSuccessPromotionNHH_YC(ID) {
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/GroupProduct/LoadGroupProduct?ID=" + ID,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessPromotionNHH_YC,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}

function OnSuccessPromotionNHH_YC(apiResponse) {
    try {
        if (apiResponse.Success) {
            var radiovalue = "";
            var nf = Intl.NumberFormat('vi-VN');
            for (var m = 0; m < apiResponse.Detail.length; m++) {
                if (apiResponse.Detail[m].Key == "NAME") {
                    document.getElementById("lblNamePromotionNHH_YC").innerHTML = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID") {
                    document.getElementById("idNhomHangHoa_YC").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_DVT") {
                    document.getElementById("lstdm_DonViTinhNHH_YC").value = apiResponse.Detail[m].Value;
                    jQuery("#lstdm_DonViTinhNHH_YC").val(apiResponse.Detail[m].Value);
                    jQuery("#lstdm_DonViTinhNHH_YC").trigger("chosen:updated");
                    radiovalue = apiResponse.Detail[m].Value;
                    continue;
                }
            }
           
            document.getElementById("QtyProductNHH_YC").value = 1;
            myFunOpen("myModalPromotionNHH_YC");
        }
        else {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else
                alert(apiResponse.Message);
        }
        CloseLoaderCategory();
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoaderCategory();
    }

}

function OnSuccessPromotion_YC(apiResponse) {
    try {
        if (apiResponse.Success) {
            var radiovalue = "";
            var nf = Intl.NumberFormat('vi-VN');
            for (var m = 0; m < apiResponse.Detail.length; m++) {
                if (apiResponse.Detail[m].Key == "NAME") {
                    document.getElementById("lblNamePromotion_YC").innerHTML = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID") {
                    document.getElementById("idProductPromotion_YC").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_DVT") {
                    document.getElementById("ID_DVTPromotion_YC").value = apiResponse.Detail[m].Value;
                    radiovalue = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_DVT_QD") {
                    if (apiResponse.Detail[m].Value != null && apiResponse.Detail[m].Value != "") {
                        document.getElementById("ID_DVT_QDPromotion_YC").value = apiResponse.Detail[m].Value;
                        document.getElementById("ID_DVT_QDPromotion_YC").style.display = "";
                    }
                    else {
                        document.getElementById("ID_DVT_QDPromotion_YC").style.display = "none";
                        document.getElementById("ID_DVT_QDPromotion_YC").value = "";
                    }
                    continue;
                }
                if (apiResponse.Detail[m].Key == "NAME_DVT") {
                    document.getElementById("lblID_DVTPromotion_YC").innerHTML = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "NAME_DVT_QD") {
                    if (apiResponse.Detail[m].Value != null && apiResponse.Detail[m].Value != "") {
                        document.getElementById("ID_DVT_QDPromotion_YC").style.display = "";
                        document.getElementById("lblID_DVT_QDPromotion_YC").innerHTML = apiResponse.Detail[m].Value;
                    }
                    else {
                        document.getElementById("ID_DVT_QDPromotion_YC").style.display = "none";
                        document.getElementById("lblID_DVT_QDPromotion_YC").innerHTML = "";
                    }

                    continue;
                }
                if (apiResponse.Detail[m].Key == "GIA") {
                    document.getElementById("PricePromotion_YC").value = apiResponse.Detail[m].Value;
                    document.getElementById("PriceProductPromotion_YC").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "GIA_QD") {
                    document.getElementById("PriceProduct_QDPromotion_YC").value = apiResponse.Detail[m].Value;
                    continue;
                }
            }
            $('input:radio[name="ID_DVT"][value=\"' + radiovalue + '\"]').prop('checked', true);
            document.getElementById("QtyProductPromotion_YC").value = 1;
            myFunOpen("myModalPromotion_YC");
            ShowHideDivPromotion_YC();
        }
        else {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else
                alert(apiResponse.Message);
        }
        CloseLoaderCategory();
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoaderCategory();
    }

}

function ShowHideDivPromotion_YC() {
    try {
        var chkYes = document.getElementById("ID_DVTPromotion_YC");
        var PriceProductCombo = document.getElementById("PriceProductPromotion_YC");
        var PriceProduct = document.getElementById("PricePromotion_YC");
        var PriceProduct_QD = document.getElementById("PriceProduct_QDPromotion_YC");
        if (chkYes.checked)
            PriceProductCombo.value = PriceProduct.value;
        else
            PriceProductCombo.value = PriceProduct_QD.value;
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
   
}


function OnSuccessProductPromotion_YC(apiResponse) {
    try {
        if (apiResponse.Success) {
            try {
                var tbodyTempItemCombo = document.getElementById('tbodyTempItemdivPromotion_YC');
                tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
            } catch { }
            try {
                var tbodyTempItemCombo = document.getElementById('tbodyTempItemdivPromotion_YCEdit');
                tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
            } catch { }
           
            myFunClosed("myModalPromotion_YC");
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


function myFunctionDeletePromotion_YC(Controller, id, id_dvt) {
    if (confirm("Bạn muốn thực hiện xóa!")) {
        $.ajax({
            type: "POST",
            url: "/" + Controller + "/DeleteProductPromotion_YC",
            data: "{ID_HANGHOA:'" + id + "', ID_DVT:'" + id_dvt + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessProductPromotion_YC,
            error: OnErrorLoadEdit
        });
    }
}

function myFunSuccessPromotion_Tang(ID, Type, ID_KHO) {
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/Product/LoadProduct?ID=" + ID + "&Type=" + Type + "&ID_KHO=" + ID_KHO,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessPromotion_Tang,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}

function OnSuccessPromotion_Tang(apiResponse) {
    try {
        if (apiResponse.Success) {
            var radiovalue = "";
            var nf = Intl.NumberFormat('vi-VN');
            for (var m = 0; m < apiResponse.Detail.length; m++) {
                if (apiResponse.Detail[m].Key == "NAME") {
                    document.getElementById("lblNamePromotion_Tang").innerHTML = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID") {
                    document.getElementById("idProductPromotion_Tang").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_DVT") {
                    document.getElementById("ID_DVTPromotion_Tang").value = apiResponse.Detail[m].Value;
                    radiovalue = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "ID_DVT_QD") {
                    if (apiResponse.Detail[m].Value != null && apiResponse.Detail[m].Value != "") {
                        document.getElementById("ID_DVT_QDPromotion_Tang").value = apiResponse.Detail[m].Value;
                        document.getElementById("ID_DVT_QDPromotion_Tang").style.display = "";
                    }
                    else {
                        document.getElementById("ID_DVT_QDPromotion_Tang").style.display = "none";
                        document.getElementById("ID_DVT_QDPromotion_Tang").value = "";
                    }
                    continue;
                }
                if (apiResponse.Detail[m].Key == "NAME_DVT") {
                    document.getElementById("lblID_DVTPromotion_Tang").innerHTML = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "NAME_DVT_QD") {
                    if (apiResponse.Detail[m].Value != null && apiResponse.Detail[m].Value != "") {
                        document.getElementById("ID_DVT_QDPromotion_Tang").style.display = "";
                        document.getElementById("lblID_DVT_QDPromotion_Tang").innerHTML = apiResponse.Detail[m].Value;
                    }
                    else {
                        document.getElementById("ID_DVT_QDPromotion_Tang").style.display = "none";
                        document.getElementById("lblID_DVT_QDPromotion_Tang").innerHTML = "";
                    }

                    continue;
                }
                if (apiResponse.Detail[m].Key == "GIA") {
                    document.getElementById("PricePromotion_Tang").value = apiResponse.Detail[m].Value;
                    document.getElementById("PriceProductPromotion_Tang").value = apiResponse.Detail[m].Value;
                    continue;
                }
                if (apiResponse.Detail[m].Key == "GIA_QD") {
                    document.getElementById("PriceProduct_QDPromotion_Tang").value = apiResponse.Detail[m].Value;
                    continue;
                }
            }
            $('input:radio[name="ID_DVT"][value=\"' + radiovalue + '\"]').prop('checked', true);
            document.getElementById("QtyProductPromotion_Tang").value = 1;
            myFunOpen("myModalPromotion_Tang");
            ShowHideDivPromotion_Tang();
        }
        else {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else
                alert(apiResponse.Message);
        }
        CloseLoaderCategory();
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoaderCategory();
    }

}

function ShowHideDivPromotion_Tang() {
    try {
        var chkYes = document.getElementById("ID_DVTPromotion_Tang");
        var PriceProductCombo = document.getElementById("PriceProductPromotion_Tang");
        var PriceProduct = document.getElementById("PricePromotion_Tang");
        var PriceProduct_QD = document.getElementById("PriceProduct_QDPromotion_Tang");
        if (chkYes.checked)
            PriceProductCombo.value = PriceProduct.value;
        else
            PriceProductCombo.value = PriceProduct_QD.value;
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
  
}

function OnSuccessProductPromotion_Tang(apiResponse) {
    try {
        if (apiResponse.Success) {
            try {
                var tbodyTempItemCombo = document.getElementById('tbodyTempItemdivPromotion_Tang');
                tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
            } catch { }
           
            try {
                var tbodyTempItemCombo = document.getElementById('tbodyTempItemdivPromotion_TangEdit');
                tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
            } catch { }
            
            myFunClosed("myModalPromotion_Tang");
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
function LockKho(name, value) {
    try {
        if (value != "") {
            document.getElementById(name + "_chosen").style.pointerEvents = "none";
        }
        else {
            document.getElementById(name + "_chosen").style.pointerEvents = "";
        }
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
   
}
function myFunctionDeletePromotion_Tang(Controller, id, id_dvt) {
    if (confirm("Bạn muốn thực hiện xóa!")) {
        $.ajax({
            type: "POST",
            url: "/" + Controller + "/DeleteProductPromotion_Tang",
            data: "{ID_HANGHOA:'" + id + "', ID_DVT:'" + id_dvt + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessProductPromotion_Tang
        });
    }
}


function myFunctionViewReport(Controller, id)
{
    try
    {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/" + Controller + "/ViewReport?ID=" + id,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessLoadViewReport,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }
} 

function myFunctionViewReportType(Controller, id, LoaiPhieuIn, SoLan) {
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/" + Controller + "/ViewReportType?ID=" + id + "&LOAIPHIEUIN=" + LoaiPhieuIn + "&SOLAN=" + SoLan,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessLoadViewReport,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }
}

function OnSuccessLoadViewReport(apiResponse)
{
    try
    {
        CloseLoaderCategory();
        if (apiResponse.Success)
        {
            if (apiResponse.URL != null && apiResponse.URL != "")
                $("#reportViewReport").attr("src", apiResponse.URL).load();
             else
                $("#reportViewReport").attr("src", "ViewReport/VerReporte1").load();

            var lblNameViewReport = document.getElementById('lblNameViewReport');
            lblNameViewReport.innerHTML = apiResponse.NAME;
            //myFunOpen("myModalViewReport");
            const myWindow = window.open('/ViewReport/GetReport', 'MyWindow', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,resizable=no,width=900,height=600')
        }
        else
        {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else
                alert(apiResponse.Message);
        }
    }
    catch (ex)
    {
        alert(ex.Message);
        alert(ex);
    }
}

function myFunSuccessDebt(Controller, ID) {
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/" + Controller + "/LoadDetail?ID=" + ID,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessLoadViewReport,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }

}

function myFunctionCompletedDelivery_Detail(Controller, ID, TRANGTHAI)
{
    try
    {
        var ThongBao = "";
        if (TINHTRANG = '0') {
            ThongBao = "giao hàng";
        }
        else {
            ThongBao = "chưa giao hàng!";
        }
        if (confirm("Bạn muốn chuyển trạng thái " + ThongBao + "!")) {
            $.ajax({
                type: "POST",
                url: "/" + Controller + "/Completed_Detail",
                data: "{ID:'" + ID + "',TRANGTHAI:'" + TRANGTHAI + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccessCompletedDelivery,
                error: OnErrorLoadEdit
            });
        }
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }

}

function myFunctionCompletedDelivery(Controller, ID, TRANGTHAI) {
    try {
        var ThongBao = "";
        if (TINHTRANG = '0') {
            ThongBao = "hoàn tất";
        }
        else {
            ThongBao = "chưa hoàn tất!";
        }
        if (confirm("Bạn muốn chuyển trạng thái " + ThongBao + "!")) {
            $.ajax({
                type: "POST",
                url: "/" + Controller + "/Completed",
                data: "{ID:'" + ID + "',TRANGTHAI:'" + TRANGTHAI + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccessCompletedDelivery,
                error: OnErrorLoadEdit
            });
        }
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }

}

function OnSuccessCompletedDelivery(apiResponse) {
    try {
        if (apiResponse.Success)
        {
            location.reload();
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

function myFunOpenCamera(Controller, ID, ID_PHIEUXUAT) {
    try {
        OpenLoaderCategory();
        $.ajax({
            type: "GET",
            url: "/" + Controller + "/GetImageDelivery?ID=" + ID + "&ID_PHIEUXUAT=" + ID_PHIEUXUAT,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessLoadOpenCamera,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }
}

function OnSuccessLoadOpenCamera(apiResponse) {
    try {
        CloseLoaderCategory();
        if (apiResponse.Success) {
            var modal = document.getElementById("idFilemanager");
            modal.innerHTML = apiResponse.CONTENT;
            var imgtagNV = document.getElementById("idFilemanagerAdd");
            imgtagNV.style.visibility = 'hidden';
            imgtagNV.style.display = 'none';
            document.getElementById("txtNameFile").innerHTML = "";
            document.getElementById("txthrefUrl").href = "/Images_sp/hinh@2x.jpg";
            document.getElementById("txtsrcUrl").src = "/Images_sp/hinh@2x.jpg";
            document.getElementById("txtCreateFile").innerHTML = "";
            var elem = document.getElementById("ID_PHIEUGIAOHANG");
            elem.value = apiResponse.ID;
            elem = document.getElementById("ID_PHIEUXUAT");
            elem.value = apiResponse.ID_PHIEUXUAT;
            //Replaces data-rel attribute to rel.
            //We use data-rel because of w3c validation issue
            jQuery('a[data-rel]').each(function () {
                jQuery(this).attr('rel', jQuery(this).data('rel'));
            });

            jQuery("a[rel^='prettyPhoto']").prettyPhoto();
            myFunOpen("myModalViewFilemanager");
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

function OnSuccessAddFileManager_Image(apiResponse) {
    try {
        CloseLoaderCategory();
        if (apiResponse.Success) {
            var modal = document.getElementById("idFilemanager");
            modal.innerHTML = apiResponse.CONTENT;
            var imgtagNV = document.getElementById("idFilemanagerAdd");
            imgtagNV.style.visibility = 'hidden';
            imgtagNV.style.display = 'none';
            document.getElementById("txtNameFile").innerHTML = "";
            document.getElementById("txthrefUrl").href = "/Images_sp/hinh@2x.jpg";
            document.getElementById("txtsrcUrl").src = "/Images_sp/hinh@2x.jpg";
            document.getElementById("txtCreateFile").innerHTML = "";
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

function GetClientReport(ControllerName) {
    window.open('/' + ControllerName +'/GetReport', "_blank");  
}; 

function OnSuccessSearchCode(apiResponse) {
    try {
        if (apiResponse.Success) {
            if (apiResponse.URL != null && apiResponse.URL != "") {
                window.location.href = apiResponse.URL;
            }
            else
            {
                if (apiResponse.Message != null && apiResponse.Message != "") {
                    alert(apiResponse.Message);
                }
            }
            
        }
        else {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else {
               
            }
        }
        CloseLoaderCategory();
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoaderCategory();
    }

}

function OnSuccessCreateTimekeeping(apiResponse) {
    try {
        if (apiResponse.Success)
        {
            alert(apiResponse.Message);
            location.reload();
        }
        else
        {
            gritter("Thông báo lỗi", apiResponse.Message);
        }
        CloseLoaderCategory();
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoaderCategory();
    }
}
function CheckIn(Controller, NGAYCONG)
{
    try
    {
        var LATITUDELONGITUDE = document.getElementById("LATITUDELONGITUDE").value;
        var MYPUBLICIPV4 = document.getElementById("MYPUBLICIPV4").value;
        $.ajax({
            type: "POST",
            url: "/" + Controller + "/CheckIn",
            data: "{NGAYCONG:'" + NGAYCONG + "',LATITUDELONGITUDE:'" + LATITUDELONGITUDE + "',MYPUBLICIPV4:'" + MYPUBLICIPV4 + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessCreateTimekeeping,
            error: OnErrorLoadEdit
        });
    }
    catch (ex)
    {
        alert(ex.Message);
        CloseLoaderCategory();
    }
}

function CheckOut(Controller,NGAYCONG,ID) {
    try
    {
        var LATITUDELONGITUDE = document.getElementById("LATITUDELONGITUDE").value;
        var MYPUBLICIPV4 = document.getElementById("MYPUBLICIPV4").value;
        $.ajax({
            type: "POST",
            url: "/" + Controller + "/CheckOut",
            data: "{NGAYCONG:'" + NGAYCONG + "',ID:'" + ID + "',LATITUDELONGITUDE: '" + LATITUDELONGITUDE + "',MYPUBLICIPV4:'" + MYPUBLICIPV4 + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessCreateTimekeeping,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        CloseLoaderCategory();
    }
}

function CallChangeEmployee(val, type)
{
    OpenLoaderCategory();
    if (val != null && val != '') {
        $.ajax({
            type: "POST",
            url: "/" + type + "/CallChangeEmployee",
            data: "{id:'" + val + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessCallChangeEmployee,
            error: OnErrorLoadEdit
        });
    }
}



function OnSuccessCallChangeEmployee(apiResponse) {
    try
    {
        
        if (apiResponse.Success)
        {
            var bolError = false;
            var bolErrorISACTIVE = false;
            var divElem = document.getElementById('myModalAdd');
            var lst = divElem.querySelectorAll("select");
            for (var i = 0; i < apiResponse.Detail.length; i++) {
                if (lst != null && lst.length > 0)
                {
                    for (var j = 0; j < lst.length; j++)
                    {
                        if (lst[j].type == "select-one" && lst[j].id == "lstnv_PhepNam") {
                            var select = lst[j];
                            var option1 = select.options[0];

                            var valuetext = null;
                            $(select).empty();
                            $(select).append(option1);
                            for (m = 0; m < apiResponse.Detail.length; m++) {
                                if (lst[j].id == apiResponse.Detail[m].Key && apiResponse.Detail[m].Value != null) {
                                    for (n = 0; n < apiResponse.Detail[m].Value.length; n++) {
                                        if (bolErrorISACTIVE == false) {
                                            try {
                                                if (apiResponse.Detail[m].Value[n].ISACTIVE != null) {
                                                    if (apiResponse.Detail[m].Value[n].ISACTIVE == true) {
                                                        $(select).append('<option value=' + apiResponse.Detail[m].Value[n].ID + '>' + apiResponse.Detail[m].Value[n].NAME + '</option>');
                                                    }
                                                }
                                                else {
                                                    $(select).append('<option value=' + apiResponse.Detail[m].Value[n].ID + '>' + apiResponse.Detail[m].Value[n].NAME + '</option>');
                                                }
                                            }
                                            catch
                                            {
                                                $(select).append('<option value=' + apiResponse.Detail[m].Value[n].ID + '>' + apiResponse.Detail[m].Value[n].NAME + '</option>');
                                                bolErrorISACTIVE = true;
                                            }
                                        }
                                        else {
                                            $(select).append('<option value=' + apiResponse.Detail[m].Value[n].ID + '>' + apiResponse.Detail[m].Value[n].NAME + '</option>');
                                        }


                                        if (bolError == false) {
                                            try {
                                                if (apiResponse.Detail[m].Value[n].ISDEFAULT == true) {
                                                    valuetext = apiResponse.Detail[m].Value[n].ID;
                                                }
                                            }
                                            catch
                                            {
                                                bolError = true;
                                            }
                                        }
                                    }
                                }
                            }
                            lst[j].value = valuetext;
                            jQuery(select).val(valuetext);
                            jQuery(select).trigger("chosen:updated");
                            $(select).val(valuetext).change();
                                
                            }
                        }
                }
            }
        }
        else {
            gritter("Thông báo lỗi", apiResponse.Message);
        }
        CloseLoaderCategory();
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoaderCategory();
    }
}

function myFunctionDeletePayroll(id, Controller) {
    if (confirm("Bạn muốn thực hiện xóa!")) {
        $.ajax({
            type: "POST",
            url: "/" + Controller +"/RemovePayroll",
            data: "{ID:'" + id + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessPayroll,
            error: OnErrorLoadEdit
        });
    }
}

function myFunctionAddPayroll(Controller) {
    $.ajax({
        type: "POST",
        url: "/" + Controller +"/AddPayroll",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccessPayroll,
        error: OnErrorLoadEdit
    });
}

function OnSuccessPayroll(apiResponse) {
    try {
        if (apiResponse.Success) {
            try {
                var tbodyTempItemCombo = document.getElementById('tbodyReport_Add');
                tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
            } catch { }
            try {
                var tbodyTempItemCombo = document.getElementById('tbodyReport_Edit');
                tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
            } catch { }
            try {
                if (apiResponse.Detail != null) {
                    for (var i = 0; i < apiResponse.Detail.length; i++)
                    {
                        var divElem = document.getElementById('myModalAdd');
                        var lstdivElem = divElem.querySelectorAll("[name^='" + apiResponse.Detail[i].Key + "'");
                        if (lstdivElem != null && lstdivElem.length > 0)
                        { 
                            lstdivElem[0].value = apiResponse.Detail[i].Value;
                        }

                        divElem = document.getElementById('myModalEdit');
                        lstdivElem = divElem.querySelectorAll("[name^='" + apiResponse.Detail[i].Key + "'");
                        if (lstdivElem != null && lstdivElem.length > 0) {
                            lstdivElem[0].value = apiResponse.Detail[i].Value;
                        }
                    }
                }
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
}

function myFunctionGetPayrollDetail(Controller, myModal) {
    try
    {
        var divElem = document.getElementById(myModal);
        var lstdivElemID_THANGLUONG = divElem.querySelectorAll("[name^='ID_THANGLUONG'");
        var ID_THANGLUONG = lstdivElemID_THANGLUONG[0].value;
        var lstdivElemID_NHANVIEN = divElem.querySelectorAll("[name^='ID_NHANVIEN'");
        var ID_NHANVIEN = lstdivElemID_NHANVIEN[0].value;

        var lstdivElemID = divElem.querySelectorAll("[name^='ID'");
        var ID = lstdivElemID[0].value;
        if (ID == '' || ID_THANGLUONG == '' || ID_NHANVIEN == '') {
            alert("Vui lòng chọn tháng lương và nhân viên cần tính lương!");
            return;
        }
        $.ajax({
            type: "POST",
            url: "/" + Controller + "/GetPayrollDetail",
            data: "{ID_THANGLUONG:'" + ID_THANGLUONG + "',ID_NHANVIEN:'" + ID_NHANVIEN + "',ID:'" + ID + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessPayroll,
            error: OnErrorLoadEdit
        });
    }
    catch (ex) {
        alert(ex.Message);
        CloseLoaderCategory();
    }
}
function OnSubmitCopyKPI_Sale() {
    try {
        if (confirm("Bạn muốn thực hiện sao chép dữ liệu!")) {
            OpenLoaderCategory();
            var cartList = [];
            var array = document.querySelectorAll('#tbodytmpitem input[name="item.CHON"]')
            if (array != null && array.length > 0)
            {
                for (var i = 0; i < array.length; i++) {
                    if (array[i].checked == true) {
                        cartList.push({
                            ID: array[i].id
                        });
                    }
                }

                if (cartList != null && cartList.length > 0)
                {
                    $.ajax({
                        url: "/KPI_Sale/OnSubmitKPI_Sale",
                        data: { cartOrder: JSON.stringify(cartList) },
                        dataType: "json",
                        success: OnSubmitCopy,
                        error: OnErrorLoadEdit
                    });
                }
                else {
                    CloseLoaderCategory();
                    alert("Vui lòng chọn ít nhất 1 chương trình!");
                }
            }
            else {
                CloseLoaderCategory();
                alert("Vui lòng chọn ít nhất 1 chương trình!");
            }
        }
    }
    catch (ex) {
        alert(ex);
    }
}

function OnSubmitCopy(data) {
    try {
        if (data.URL != null && data.URL != "") { window.location.href = data.URL; }
        else {
            if (data.Message != null && data.Message != "") { alert(data.Message); }
            location.reload();
        }
        CloseLoaderCategory();
    }
    catch
    {
        CloseLoaderCategory();
    }

}

function myFunctionPopupImage(Controller, id) {
    if (confirm("Bạn muốn thực hiện xóa!")) {
        $.ajax({
            type: "POST",
            url: "/" + Controller + "/DeletePopup",
            data: "{id:'" + id + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessDeleteImage,
            error: OnErrorLoadEdit
        });
    }
}
function OnSuccessDeleteImage(apiResponse) {
    try {
        if (apiResponse.Success) {
            var divElement = document.querySelector('div.image#' + apiResponse.ID);
            if (divElement)
            {
                divElement.parentNode.removeChild(divElement);
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

// Gán hàm vào window
window.myFunClosed = myFunClosed;
// Gán hàm vào window
window.gritter = gritter;
// Gán hàm vào window
window.CloseLoaderCategory = CloseLoaderCategory;