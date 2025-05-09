//<!-- Khách hàng -->
//<script language="javascript" type="text/javascript">
function myAddCustomer(type) {
    alert(type)
    var objCustomer = new Object();
    objCustomer.NAME = document.getElementById("NAME").value;
    objCustomer.ADDRESS = document.getElementById("ADDRESS").value;
    objCustomer.TEL = document.getElementById("TEL").value;
    objCustomer.FAX = document.getElementById("FAX").value;
    objCustomer.EMAIL = document.getElementById("EMAIL").value;
    objCustomer.NGAYSINH = document.getElementById("NGAYSINH").value;
    //objCustomer.NGHENGHIEP = document.getElementById("NGHENGHIEP").value;
    objCustomer.DIS = document.getElementById("DIS").value;
    objCustomer.RATE = document.getElementById("RATE").value;
    objCustomer.GROUP_NHOM = document.getElementById("GROUP_NHOM").value;
    objCustomer.MAX_CONGNO = document.getElementById("MAX_CONGNO").value;
    objCustomer.SONGAY = document.getElementById("SONGAY").value;
    objCustomer.MAHANG_KH_LK = document.getElementById("MAHANG_KH_LK").value;
    objCustomer.CUM_ID = document.getElementById("CUM_ID").value;
    objCustomer.ID = document.getElementById("ID").value;
    var strtype = "AddCustomer";

    if (type == "2")
        strtype = "EditCustomer";

    $.ajax({
        type: "POST",
        url: "/Admin/" + strtype,
        //data: "{id:'" + page + "'}",
        //data: $('#myForm').serialize(),
        data: JSON.stringify(objCustomer),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccessKhachHang,
        error: OnError
    });
}

function OnSuccessKhachHang(data) {
    alert(data.THONGBAO);
}
//</script>

//<!-- Load trang khách hàng -->
//<script language="javascript" type="text/javascript">
    function myFunctionPage(page) {
        $('#progress').show();
        document.getElementById("page").value = page;
        $.ajax({
            type: "POST",
            url: "/Admin/Customer",
            data: $('#myForm').serialize(),
            dataType: "json",
            success: OnSuccess,
            error: OnError
        });
    }

function OnSuccess(data) {
    $('#progress').hide();
    $("#myData").html(data.DATA);
    $("#myPhanTrang").html(data.CHUOIPHANTRANG);
}

function OnError(data) {
    $('#progress').hide();
    alert("Lỗi: vui lòng liên hệ admin! Xin cảm ơn.");
}
//</script>

//<!-- Xóa khách hàng -->
//<script>
function myFunction(id) {
    if (confirm("Bạn muốn thực hiện xóa!")) {
        alert(id);
        $.post("/Admin/Delete_product", { "id": id },
            function (data) {
                location.reload();
            });
    }
}

//<!-- Load khách hàng -->
//<script>
function myFunctionLoad(id) {
    $.ajax({
        type: "POST",
        url: "/Admin/LoadCustomer",
        data: "{id:'" + id + "'}",
        //data: $('#myForm').serialize(),
        //data: JSON.stringify(objCustomer),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccessLoadKhachHang,
        error: OnError
    });
}
function OnSuccessLoadKhachHang(objCustomer) {
    document.getElementById("NAME").value = objCustomer.NAME;
    document.getElementById("ADDRESS").value = objCustomer.ADDRESS;
    document.getElementById("TEL").value = objCustomer.TEL;
    document.getElementById("FAX").value = objCustomer.FAX;
    document.getElementById("EMAIL").value = objCustomer.EMAIL;
    document.getElementById("NGAYSINH").value = objCustomer.NGAYSINH;
    //document.getElementById("NGHENGHIEP").value = objCustomer.NGHENGHIEP;
    document.getElementById("DIS").value = objCustomer.DIS;
    document.getElementById("RATE").value = objCustomer.RATE;
    document.getElementById("GROUP_NHOM").value = objCustomer.GROUP_NHOM;
   

    if (objCustomer.TENNHOM != null) {
        $("[id*=GROUP_NHOM_chosen] a span").each(function () {
            $(this).text(objCustomer.TENNHOM);
        });
    }
    else {
        $("[id*=GROUP_NHOM_chosen] a span").each(function () {
            $(this).text("Chọn nhóm");
        });
    }

    document.getElementById("MAX_CONGNO").value = objCustomer.MAX_CONGNO;
    document.getElementById("SONGAY").value = objCustomer.SONGAY;
    document.getElementById("MAHANG_KH_LK").value = objCustomer.MAHANG_KH_LK;
    document.getElementById("CUM_ID").value = objCustomer.CUM_ID;
    if (objCustomer.TENKHUVUC != null)
    {
        $("[id*=CUM_ID_chosen] a span").each(function () {
            $(this).text(objCustomer.TENKHUVUC);
        });
    }
    else
    {
        $("[id*=CUM_ID_chosen] a span").each(function () {
            $(this).text("Chọn khu vực");
        });
    }
    
    document.getElementById("ID").value = objCustomer.ID;
    document.getElementById("ID").disabled = true;
    document.getElementById("idBtnSubmit").onclick = function () { myAddCustomer(2) };
    document.getElementById("lblTieuDe").innerHTML = "THÔNG TIN KHÁCH HÀNG";
    var modal = document.getElementById("myModal");
    modal.style.display = "block";
}
//</script>


//<!-- @Utility.Dong mở form loading -->
//<script>
 function myFunOpen()
{
     document.getElementById("lblTieuDe").innerHTML = "THÊM KHÁCH HÀNG";
     document.getElementById("NAME").value = "";
     document.getElementById("ADDRESS").value = "";
     document.getElementById("TEL").value = "";
     document.getElementById("FAX").value = "";
     document.getElementById("EMAIL").value = "";
     document.getElementById("NGAYSINH").value = "";
     //document.getElementById("NGHENGHIEP").value = "";
     document.getElementById("DIS").value = "";
     document.getElementById("RATE").value = "";
     document.getElementById("GROUP_NHOM").value = "";
     document.getElementById("MAX_CONGNO").value = "";
     document.getElementById("SONGAY").value = "";
     document.getElementById("MAHANG_KH_LK").value = "";
     document.getElementById("CUM_ID").value = "";
     document.getElementById("ID").value = "";
     document.getElementById("ID").disabled = true;
     document.getElementById("idBtnSubmit").onclick = function () { myAddCustomer(1) };
     var modal = document.getElementById("myModal");
     modal.style.display = "block";
 }

 function myFunClosed() {
    var modal = document.getElementById("myModal");
    modal.style.display = "none";
}
//</script>

//<script>
/* When the user clicks on the button,
toggle between hiding and showing the dropdown content */
function myFunction() {
    document.getElementById("myDropdown").classList.toggle("show");
}

function filterFunction() {
    var input, filter, ul, li, a, i;
    input = document.getElementById("myInput");
    filter = input.value.toLowerCase();
    div = document.getElementById("myDropdown");
    a = div.getElementsByTagName("a");
    for (i = 0; i < a.length; i++) {
        txtValue = a[i].textContent || a[i].innerText;
        var str = txtValue.toLowerCase().replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a").replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e").replace(/ì|í|ị|ỉ|ĩ/g, "i").replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o").replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u").replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y").replace(/đ/g, "d");
        //alert(str);
        var n = str.search(filter);
        if (n > -1) {
            a[i].style.display = "";
        } else {
            a[i].style.display = "none";
        }
    }
}
//</script>

//<!-- @Utility.Dong mở form loading -->
//<script>
// Get the modal
var modal = document.getElementById("myModal");

// Get the button that opens the modal
var btn = document.getElementById("myBtn");

// Get the <span> element that closes the modal
var span = document.getElementsByClassName("close")[0];

// When the user clicks the button, open the modal
btn.onclick = function() {
    modal.style.display = "block";
}

// When the user clicks on <span> (x), close the modal
span.onclick = function() {
    modal.style.display = "none";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function(event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}
//</script>