// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let App = {};

//setup
App.datePicker = $("#datePicker");
App.buyTable = $("#buyTable");
App.sellTable = $("#sellTable");
App.ticketTable = $("#ticketTable");
App.bestTable = $("#bestTable");
App.recomendTable = $("#recomendTable");

App.bankSelect = $("#bankSelect");
App.datePicker = $("#datePicker");

App.buyTableBtn = $("#buyTableBtn");
App.sellTableBtn = $("#sellTableBtn");
App.ticketTableBtn = $("#ticketTableBtn");
App.recomendTableBtn = $("#recomendTableBtn");
App.bestTableBtn = $("#bestTableBtn");

App.allTables = $(".tbl");
App.deleteBtn = $("#deleteAll");

let date = new Date();
let day = `${date.getUTCDay()}`.padStart(2, "0");
let month = `${date.getUTCMonth() + 1}`.padStart(2, "0");
App.datePicker.val(`${date.getFullYear()}-${month}-${day}`);

console.log($(App.bankSelect).val());

//event handlers


App.deleteBtn.on('click', () => {
    for (let table of App.allTables) {
        $(table).empty();
    }
});



App.recomendTableBtn.on('click', () => {
    $.ajax({
        type: 'GET',
        url: '/ExchangeRate/RecomendationTable',
        data: {
            tableDate: App.datePicker.val()
        },
        cache: false,
        success: function (result) {
            $(App.recomendTable).append(result);
        },
        error: function (xhr, status, error) {
            Console.log(error);
        }
    });
});

App.bestTableBtn.on('click', () => {
    $.ajax({
        type: 'GET',
        url: '/ExchangeRate/BestOfDateTable',
        data: {
            tableDate: App.datePicker.val()
        },
        cache: false,
        success: function (result) {
            $(App.bestTable).append(result);
        },
        error: function (xhr, status, error) {
            Console.log(error);
        }
    });
});


App.buyTableBtn.on('click', () => {

    $.ajax({
        type: 'GET',
        url: '/ExchangeRate/BuyTable',
        data: {
            tableDate: App.datePicker.val()
        },
        cache: false,
        success: function (result) {
            $(App.buyTable).append(result);
        },
        error: function (xhr, status, error) {
            Console.log(error);
        }
    });
});

App.sellTableBtn.on('click', () => {
    $.ajax({
        type: 'GET',
        url: '/ExchangeRate/SellTable',
        data: {
            tableDate: App.datePicker.val()
        },
        cache: false,
        success: function (result) {
            $(App.sellTable).append(result);
        },
        error: function (xhr, status, error) {
            Console.log(error);
        }
    });
});

App.ticketTableBtn.on('click', () => {
    $.ajax({
        type: 'GET',
        url: '/ExchangeRate/TicketTable',
        data: {
            bankName: $(App.bankSelect).val() ,
            tableDate: App.datePicker.val()
        },
        cache: false,
        success: function (result) {
            $(App.recomendTable).append(result);
        },
        error: function (xhr, status, error) {
            Console.log(error);
        }
    });
});





    
