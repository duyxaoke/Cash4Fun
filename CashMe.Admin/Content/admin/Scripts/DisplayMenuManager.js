$(document).ready(function () {

    //custom column table
    var columnDefs = [{ "width": "200px", "targets": 0 }, {
        "targets": [0],
        orderable: false,
        render: function (data, type, row) {
            var html = '<div class="btn-group">';
            html += '<a href="#" role="button" data-id=' + data + ' onclick="javascript: getMenuById(this);" data-toggle="modal" data-target="#detail_modal" class="btn btn-primary btn-embossed btn-sm"><i class="fa fa-edit"></i> Sửa</a>';
            html += '<a href="#" role="button" onclick="javascript: deleteCategoriesById(' + data + ');"  class="btn btn-warning btn-embossed btn-sm"><i class="fa fa-remove"></i> Xóa</a>';
            html += '</div>';
            return html;
        }
    },
    {
        //"targets": [4],
        //render: function (data, type, row) {
        //    if (data == 1)
        //        return '<span class="label label-primary">Active</span>';
        //    return '<span class="label label-default">Inactive</span>';
        //}
    }];
    //render datatable
    var oTable = sys.Paging($('#tbMenu'), "/DisplayMenu/AjaxHandler", columnDefs);

    //submit modal Category
    $('#frmMenu').submit(function () {
        if ($(this).valid()) {
            sys.Loading();
            var Id = $("#frmMenu input[name=Id]").val(),
                Name = $("#frmMenu input[name=Name]").val(),
                IsActive = $("#frmMenu input[name=IsActive]").val(),
                LinkedController = $("#frmMenu input[name=LinkedController]").val(),
                RoleId = $("#frmMenu select[name=RoleId]").find("option:selected").val();
              
            //var Flag = 1;
            //$("#frmUsers input[name=Flag]").each(function () {
            //    if ($(this).is(':checked')) {
            //        Flag = $(this).val();
            //    }
            //});
          
            var obj = new Object();
            obj.Id = Id;
            obj.Name = Name;
            obj.IsActive = IsActive;
            obj.LinkedController = LinkedController;
            obj.RoleId = RoleId;

            $.when(sys.CallAjaxPost("/DisplayMenu/Modify", obj)).done(function (data) {
                if (data.Status == "Success") {
                    $('#detail_modal').modal('toggle');
                    oTable.fnDraw();
                    sys.Alert("Lưu thành công");
                }
                else {
                    sys.Error("Lưu lại thất bại!");
                }
            });
            sys.HideLoading();
        }
        event.preventDefault();

    });

    

});

function getMenuById(obj) {
    
    var id = $(obj).attr("data-id");
    sys.resetForm("#frmMenu");
    $('#frmMenu #selectrole option:selected').prop('selected', false);
    $('#frmMenu select[name=RoleId]').change();
    
    //kiem tra neu la update thi show thông tin, nguoc lai thi clear form
    if (id !== 0) {
        $.when(sys.CallAjaxPost("/DisplayMenu/GetMenuById", { id: id })).done(function (data) {
            if (sys.checkNull(data)) {
                $("#frmMenu input[name=Id]").val(data.Id);
                $("#frmMenu input[name=Name]").val(data.Name);
                $("#frmMenu input[name=LinkedController]").val(data.LinkedController);
                $('#frmMenu select[name=RoleId]').val(data.RoleId);
                $('#frmMenu select[name=RoleId]').change();
                $('#frmMenu input[name=IsActive]').iCheck('update');
                if (data.IsActive == 1)
                {
                    $('#frmMenu input[name=IsActive]').iCheck('check');
                }
                else
                {
                    $('#frmMenu input[name=IsActive]').iCheck('uncheck');
                }
            }
        });
    }
}

//function deleteCategoriesById(id) {
//    // disable
//    sys.ConfirmDialog(function () {
//        $.when(sys.CallAjaxPost("/Categories/DeleteCategories", { id: id })).done(function (data) {
//            if (data.Status == "Success") {
//                $('#tbCategories').dataTable().fnDraw();
//                sys.Alert("success");
//            } else {
//                sys.Alert("error");
//            }
//        });
//    });
//}
