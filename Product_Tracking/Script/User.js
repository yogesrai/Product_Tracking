// <reference path="jquery-3.3.1.intellisense.js" />

function Add() {

    var userobj =
    {
        UserId: $('#userid').val(),
        UserName: $('#username').val(),
        Email: $('#email').val(),
        PhoneNumber: $('#phonenumber').val(),
        address: $('#address').val(),
        //RoleId:1,
        Password: $('#password').val()
    };
    $.ajax({
        url: "/Account/AddUser",
        data: JSON.stringify(userobj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {            
                $('#myModal').modal('hide');
                alert("User Created Successfully!")                       
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
function login() {
    var userdetail = {
        username: $('#Uname').val(), 
        password: $('#Pname').val()
    }
    $.ajax({
        url: "/Account/Login",
        data: JSON.stringify(userdetail),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result==true) {
                window.location.href = 'Admin/Index';             
            }
            else {
                window.location.href = 'Login';     
            }
            
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}