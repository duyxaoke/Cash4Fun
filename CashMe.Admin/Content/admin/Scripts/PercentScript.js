$(document).ready(function () {
    //chan nhap chu
    sys.blockword($("#frmPercent input[name=Value]"));

    //custom column table
    var columnDefs = [{ "width": "200px", "targets": 0 }, {
        "targets": [0],
        orderable: false,
        render: function (data, type, row) {
            var html = '<div class="btn-group">';
            html += '<a href="#" role="button" onclick="javascript: getPercentById(' + data + ');" data-toggle="modal" data-target="#detail_modal" class="btn btn-primary btn-embossed btn-sm"><i class="fa fa-edit"></i> Sửa</a>';
            html += '<a href="#" role="button" onclick="javascript: deletePercentById(' + data + ');"  class="btn btn-warning btn-embossed btn-sm"><i class="fa fa-remove"></i> Xóa</a>';
            html += '</div>';
            return html;
        }
    },
    {
        "targets": [4],
        render: function (data, type, row) {
            if (data == 1)
                return '<span class="label label-primary">Active</span>';
            return '<span class="label label-default">Inactive</span>';
        }
    }];
    //render datatable
    var oTable = sys.Paging($('#tbPercent'), "/Percent/AjaxHandler", columnDefs);



    //submit modal Category
    $('#frmPercent').submit(function () {
        if ($(this).valid()) {
            sys.Loading();
            var Id = $("#frmPercent input[name=Id]").val(),
            Value = $("#frmPercent input[name=Value]").val();
            var Flag = 1;
            $("#frmPercent input[name=Flag]").each(function () {
                if ($(this).is(':checked')) {
                    Flag = $(this).val();
                }
            });
            var obj = new Object();
            obj.Id = Id;
            obj.Value = Value;
            obj.Flag = Flag;

            $.when(sys.CallAjaxPost("/Percent/Modify", obj)).done(function (data) {
                if (data.Status == "Success") {
                    $('#detail_modal').modal('toggle');
                    oTable.fnDraw();
                    sys.Alert("success");
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

function getPercentById(id) {
    sys.resetForm("#frmPercent");
    //kiem tra neu la update thi show thông tin, nguoc lai thi clear from
    if (id !== 0) {
        $.when(sys.CallAjaxPost("/Percent/getPercentById", { id: id })).done(function (data) {
            if (sys.checkNull(data)) {
                $("#frmPercent input[name=Id]").val(data.Id);
                $("#frmPercent input[name=Value]").val(data.Value);
                $("#frmPercent input[name=Flag]").parent().removeClass('checked');
                $("#frmPercent input[name=Flag]").removeAttr("checked");
                $("#frmPercent input[name=Flag][value=" + data.Flag + "]").parent().addClass('checked');
                $("#frmPercent input[name=Flag][value=" + data.Flag + "]").attr('checked', 'checked');
            }
        });
    }
}

function deletePercentById(id) {
    // disable
    sys.ConfirmDialog(function () {
        $.when(sys.CallAjaxPost("/Percent/DeletePercent", { id: id })).done(function (data) {
            if (data.Status == "Success") {
                $('#tbPercent').dataTable().fnDraw();
                sys.Alert("success");
            } else {
                sys.Alert("error");
            }
        });
    });
}
