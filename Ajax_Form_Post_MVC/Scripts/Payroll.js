function CallChangePayroll(val, type)
{
    if (val != null && val != '')
    {
        $.ajax({
            type: "POST",
            url: "/Payroll/CallChangePayroll",
            data: "{id:'" + val + "',type:'" + type + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessCallChangePayroll,
            error: OnErrorPayroll
        });
    }
}

function OnSuccessCallChangePayroll(data)
{
    if (data != null && data.DataObject != null)
    {
        if (data.DATA == 'dm_ThangLuong')
        {
            var x = document.getElementsByName("SONGAYCONG");
            if (x != null && x.length > 0) {
                for (var i = 0; i < x.length; i++) {
                    x[i].value = data.DataObject.SONGAYCONG;
                }
            }

            x = document.getElementsByName("SONGAYLAMVIEC");
            if (x != null && x.length > 0) {
                for (var i = 0; i < x.length; i++) {
                    x[i].value = data.DataObject.SONGAYCONG;
                }
            }
        }

        if (data.DATA == 'dm_NhanVien') {
            var x = document.getElementsByName("MUCLUONG");
            if (x != null && x.length > 0) {
                for (var i = 0; i < x.length; i++) {
                    x[i].value = data.DataObject.LUONGCOBAN;
                }
            }

            var x = document.getElementsByName("TIENLUONG");
            if (x != null && x.length > 0) {
                for (var i = 0; i < x.length; i++) {
                    x[i].value = data.DataObject.LUONGCOBAN;
                }
            }
        }


        if (data.DATA == 'NGAYLAP') {
            var x = document.getElementsByName("MAPHIEU");
            if (x != null && x.length > 0) {
                for (var i = 0; i < x.length; i++) {
                    x[i].value = data.DataObject.MAPHIEU;
                }
            }

            x = document.getElementsByName("SOPHIEU");
            if (x != null && x.length > 0) {
                for (var i = 0; i < x.length; i++) {
                    x[i].value = data.DataObject.SOPHIEU;
                }
            }
        }
    }
    else if (data.URL != null && data.URL != "")
        window.location.href = data.URL;
}

function OnErrorPayroll(data)
{
    alert("Lỗi: vui lòng liên hệ! Xin cảm ơn.");
}

function OnSuccessCreatePayroll(apiResponse)
{
    try
    {
        if (apiResponse.Success)
        {
            myFunClosed("myModalAdd");
            var i, j, m = 0;
            var array = $("#tbodytmpitem tr[id=\"" + apiResponse.ID + "\"] td");
            if (array != null && array.length > 0) {
                for (m = 0; m < array.length; m++) {
                    var lst = array[m].querySelectorAll("button");
                    if (lst != null && lst.length > 0) {
                        for (i = 0; i < apiResponse.Detail.length; i++)
                        {

                            for (j = 0; j < lst.length; j++) {
                                if (lst[j].id == apiResponse.Detail[i].Key) {
                                    if (lst[j].localName == "button") {
                                        lst[j].outerHTML = apiResponse.Detail[i].Value;
                                        //lst[j].attributes[2].nodeValue = "myFunctionEdit('Timekeeping','" + apiResponse.MAPHIEU + "')";
                                    }
                                    else
                                        lst[j].value = apiResponse.Detail[i].Value;

                                    continue;
                                }

                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else {
                if (apiResponse.CheckValue)
                {
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
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoaderCategory();
    }
}

function OnSuccessEditPayroll(apiResponse) {
    try {
        if (apiResponse.Success) {
            var i, j, m = 0;
            var array = $("#tbodytmpitem tr[id=\"" + apiResponse.ID + "\"] td");
            if (array != null && array.length > 0) {
                for (m = 0; m < array.length; m++) {
                    var lst = array[m].querySelectorAll("button");
                    if (lst != null && lst.length > 0) {
                        for (i = 0; i < apiResponse.Detail.length; i++) {

                            for (j = 0; j < lst.length; j++) {
                                if (lst[j].id == apiResponse.Detail[i].Key) {
                                    if (lst[j].localName == "button")
                                        lst[j].outerHTML = apiResponse.Detail[i].Value;
                                    else
                                        lst[j].value = apiResponse.Detail[i].Value;

                                    continue;
                                }

                            }
                        }
                    }
                }
            }
            myFunClosed("myModalEdit");
        }
        else {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else {
                if (apiResponse.Data != null) {
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
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoaderCategory();
    }
}