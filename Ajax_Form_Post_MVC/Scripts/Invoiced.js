
function OpenLoader() {
    try {
        var modal = document.getElementById("myModal1");
        modal.style.display = "block";
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}

function CloseLoader() {
    try {
        var modal = document.getElementById("myModal1");
        modal.style.display = "none";
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}

function OnSubmitInvoiced() {
    try {
        var e = document.getElementById("HINHTHUC_THUCHIEN");
        var value = e.value;
        var text = e.options[e.selectedIndex].text;
        if (confirm("Bạn muốn thực hiện tạo hóa đơn " + text + "!")) {
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
                        type: "POST",
                        url: "/Invoiced/OnSubmitDeposit",
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
    try {
        if (data.URL != null && data.URL != "") { window.location.href = data.URL; }
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

function OnSubmitInvoicedList() {
    try {
        if (confirm("Bạn muốn thực hiện tạo hóa đơn!")) {
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
                        type: "POST",
                        url: "/Invoiced/InvoicedList",
                        data: { cartOrder: JSON.stringify(cartList)},
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

function OnSubmitGetInvoicedList(ID)
{
    try {
        OpenLoader();
        var cartList = [];
        cartList.push({
            ID: ID
        });
        $.post("/Invoiced/GetInvoicedList", { cartOrder : JSON.stringify(cartList) }, function (apiResponse) {
            try {
                CloseLoader();
                if (apiResponse.Success) {
                    if (ID == '-1' && confirm(apiResponse.Success)) {
                        
                    }
                    location.reload();
                }
                else
                {
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
        alert(ex);
    }
}
function OnSubmitDeleteInvoicedList() {
    try {
        if (confirm("Bạn muốn thực hiện xóa!")) {
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
                        type: "POST",
                        url: "/Invoiced/DeleteInvoicedList",
                        data: { cartOrder: JSON.stringify(cartList) },
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

function updateInvoiced(ID, elem, myBtn) {
    const isChecked = document.getElementById("myCheckbox").checked
    const TONGTIENGIAMGIA = $('#' + myBtn + ' input[type="number"][id="TONGTIENGIAMGIA"]').val() || 0;
    //const TONGTIENGIAMGIA = document.querySelector('#' + myBtn +' #TONGTIENGIAMGIA').value;
    const TONGTHANHTIEN = $('#' + myBtn + ' input[type="number"][id="TONGTHANHTIEN"]').val() || 0;
    const TONGTIENVAT = $('#' + myBtn + ' input[type="number"][id="TONGTIENVAT"]').val() || 0;
    const TONGTIEN = $('#' + myBtn + ' input[type="number"][id="TONGTIEN"]').val() || 0;
    //const TONGTHANHTIEN = document.querySelector('#' + myBtn +' #TONGTHANHTIEN').value;
    //const TONGTIENVAT = document.querySelector('#' + myBtn +' #TONGTIENVAT').value;
    //const TONGTIEN = document.querySelector('#' + myBtn +' #TONGTIEN').value;

    $.post("/Invoiced/UpdateProductDeposit_Temp", { "ID": ID, "TYPE": elem.id, "VALUE": elem.value, 'bolTinhLai': isChecked, 'TONGTIEN': TONGTIEN, 'TONGTIENGIAMGIA': TONGTIENGIAMGIA, 'TONGTHANHTIEN': TONGTHANHTIEN, 'TONGTIENVAT': TONGTIENVAT, 'MYBTN': myBtn }, function (apiResponse) {
        try {
            if (apiResponse.Success) {

                try {
                    var tbodyTempItemCombo = document.getElementById('tbodyTempItemInvoiced');
                    tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
                } catch { }

                try {
                    var tbodyTempItemCombo = document.getElementById('tbodyTempItemInvoicedEdit');
                    tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
                } catch { }

                $("input.form-control.maskinput").each((i, ele) => {
                    $(ele).removeAttr("onkeyup"); // Xoá thuộc tính gây sự kiện
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

function AddInvoiced(myBtn) {
    const TONGTIENGIAMGIA = $('#' + myBtn + ' input[type="number"][id="TONGTIENGIAMGIA"]').val() || 0;
    const TONGTHANHTIEN = $('#' + myBtn + ' input[type="number"][id="TONGTHANHTIEN"]').val() || 0;
    const TONGTIENVAT = $('#' + myBtn + ' input[type="number"][id="TONGTIENVAT"]').val() || 0;
    const TONGTIEN = $('#' + myBtn + ' input[type="number"][id="TONGTIEN"]').val() || 0;

    $.post("/Invoiced/AddInvoiced", { 'TONGTIEN': TONGTIEN, 'TONGTIENGIAMGIA': TONGTIENGIAMGIA, 'TONGTHANHTIEN': TONGTHANHTIEN, 'TONGTIENVAT': TONGTIENVAT, 'MYBTN': myBtn }, function (apiResponse) {
        try {
            if (apiResponse.Success) {

                try {
                    var tbodyTempItemCombo = document.getElementById('tbodyTempItemInvoiced');
                    tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
                } catch { }

                try {
                    var tbodyTempItemCombo = document.getElementById('tbodyTempItemInvoicedEdit');
                    tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
                } catch { }

                $("input.form-control.maskinput").each((i, ele) => {
                    $(ele).removeAttr("onkeyup"); // Xoá thuộc tính gây sự kiện
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

function myFunctionDeleteProdcutInvoiced(Controller, id, myBtn) {
    if (confirm("Bạn muốn thực hiện xóa!")) {
        const isChecked = document.getElementById("myCheckbox").checked
        const TONGTIENGIAMGIA = $('#' + myBtn + ' input[type="number"][id="TONGTIENGIAMGIA"]').val() || 0;
        const TONGTHANHTIEN = $('#' + myBtn + ' input[type="number"][id="TONGTHANHTIEN"]').val() || 0;
        const TONGTIENVAT = $('#' + myBtn + ' input[type="number"][id="TONGTIENVAT"]').val() || 0;
        const TONGTIEN = $('#' + myBtn + ' input[type="number"][id="TONGTIEN"]').val() || 0;
        $.post("/" + Controller + "/DeleteProductInputOutput", { 'ID': id, 'bolTinhLai': isChecked, 'TONGTIEN': TONGTIEN, 'TONGTIENGIAMGIA': TONGTIENGIAMGIA, 'TONGTHANHTIEN': TONGTHANHTIEN, 'TONGTIENVAT': TONGTIENVAT, 'MYBTN': myBtn }, function (apiResponse) {
            try {
                if (apiResponse.Success) {

                    try {
                        var tbodyTempItemCombo = document.getElementById('tbodyTempItemInvoiced');
                        tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
                    } catch { }

                    try {
                        var tbodyTempItemCombo = document.getElementById('tbodyTempItemInvoicedEdit');
                        tbodyTempItemCombo.innerHTML = apiResponse.ProductCombo;
                    } catch { }

                    $("input.form-control.maskinput").each((i, ele) => {
                        $(ele).removeAttr("onkeyup"); // Xoá thuộc tính gây sự kiện
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
}