// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let App = {};
App.TableController = "TicketTable";
App.GraphController = "TicketGraph"; 
App.ExchangeRateController = "ExchangeRate";
App.SamllGraphWidth = 350;
App.overviewMaxWidth = 734;
App.activeOverviews = {}

const arrAvg = arr => arr.reduce((a, b) => a + b, 0) / arr.length
const arrSum = arr => arr.reduce((a, b) => a + b, 0)
const arrMax = arr => Math.max(...arr);
const arrMin = arr => Math.min(...arr);
//setup


App.restoreFromSession = function (targetElement, json) {

    for (let settings of json) {

        App.getOverviewControl(targetElement, settings.currency, settings.interval, settings.isBuy);
    }
    //App.activeOverviews = json;
    //App.refreshOverviews(targetElement);
}

App.updateSession = function () {
    $.ajax({
        type: 'POST',
        contentType: "application/json",
        url: `/${App.ExchangeRateController}/UpdateSession`,
        data: JSON.stringify(Object.values(App.activeOverviews)),
        /*data: {
            data: JSON.stringify(Object.values(App.activeOverviews))
        },*/
        error: err => console.log(err),
    });
}

App.refreshOverviews = function (targetElement) {
    for (let key of Object.keys(App.activeOverviews)) {

        settings = App.activeOverviews[key];
        App.getOverviewControl(targetElement, settings.currency, settings.interval, settings.isBuy);
        App.removeOverviewControl(key);
        //App.getOverviewControl(targetElement)
    }
}



App.addActiveOverview = function (elementId,canvasId,currency, interval, isBuyText) {
    let isBuy = false;
    if (isBuyText === "True") isBuy = true;

    App.activeOverviews[elementId] = { currency: currency, interval: interval, isBuy: isBuy, canvasId: canvasId };
}

App.removeOverviewControl = function(elementId){

    delete App.activeOverviews[elementId];
    $('#' + elementId).remove();
}


App.getOverviewControl = function (targetElement,currency,interval,isBuy) {

    let length = Number(interval);
    let dateNow = new Date();

    let endDate = new Date(dateNow.getFullYear(), dateNow.getMonth(), dateNow.getDate(), 23, 59, 59);
    let startDate = new Date(endDate.getTime());

    startDate = new Date( startDate.setDate(endDate.getDate() - length));

    
    $.get({
        url: `/${App.ExchangeRateController}/OverviewControl`,
        data: {
            isoName: currency,
            interval: length,
            isBuy: isBuy
        },
        success: function (result) {

            $.get({
                url: `/${App.GraphController}/CurrencyTimelineGraphData`,
                data: {
                    currency: currency,
                    start: App.getFormatedDate(startDate),
                    end: App.getFormatedDate(endDate),
                    isBuy: isBuy
                },
                success: function (data) {
                    var elements = $(result);
                    var chr = elements.find('canvas');
                    $(window).resize(function () { App.updateOverviewSize(targetElement, chr.parent()); });

                    targetElement.append(elements).fadeIn(300, "linear");
                    App.createBankPriceTimelineGraph(chr[0], data);
                },
                error: function (err) {
                    console.log(`failed to get a graph data:\n${err}`);
                }

            });

            

            
        },
        error: function (err) {
            console.log(`failed to get a overview control:\n${err}`);
        }

    });
}

App.setDatePickerDefaultDate = function(date,targetDatepicker){

    targetDatepicker.val(App.getFormatedDate(date));
}

App.getFormatedDate = function (date) {
    let day = `${date.getUTCDate()}`.padStart(2, "0");
    let month = `${date.getUTCMonth() + 1}`.padStart(2, "0");
    return `${date.getFullYear()}-${month}-${day}`;
}

App.PrettyPrintDate = function(date){
    let day = `${date.getUTCDate()}`.padStart(2, "0");
    let month = `${date.getUTCMonth() + 1}`.padStart(2, "0");
    return `${day}. ${month}. ${date.getFullYear()}`;
}

App.getTicketPanel = function (includeBank, targetElement,callback) {

    var jqxhr = $.get({
        url: "/ExchangeRate/TicketPanel",
        data: { includeBank: includeBank },
        success: function (data) {
            var ticketPanel = targetElement//$('#ticketPanel');
            ticketPanel.fadeOut(400, 'linear', function () {
                ticketPanel.toggleClass("ticketPanelPlaceholder", false);
                ticketPanel.html(data).fadeIn(400, 'linear');
                if (callback) {
                    callback();
                }
            });
        },
        error: function () {
            console.log("Failed to get ticket panel");
        }
    });
}

App.getBuyTicket = function (targetElement,date) {
    $.get({
        url: `/${App.TableController}/BuyTable`,
        data: {

            tableDate: date
        },
        success: function (result) {
            targetElement.append(result).fadeIn(300, "linear");
        },
        error: function (err) {
            console.log(`failed to get a buy ticket:\n${err}`);
        }

    });
}

App.getSellTicket = function (targetElement, date) {
    $.get({
        url: `/${App.TableController}/SellTable`,
        data: {

            tableDate: date
        },
        success: function (result) {
            targetElement.append(result).fadeIn(300, "linear");
        },
        error: function (err) {
            console.log(`failed to get a sell ticket:\n${err}`);
        }

    });
}

App.getBankTicket = function (targetElement, date,bankName) {
    $.get({
        url: `/${App.TableController}/TicketTable`,
        data: {
            bankName: bankName,
            tableDate: date
        },
        success: function (result) {
            targetElement.append(result).fadeIn(300, "linear");
        },
        error: function (err) {
            console.log(`failed to get a sell ticket:\n${err}`);
        }

    });
}

App.getRecommendationTicket = function (targetElement, date){
    $.get({
        url: `/${App.TableController}/RecommendationTable`,
        data: {
            tableDate: date
        },
        success: function (result) {
            targetElement.append(result).fadeIn(300, "linear");
        },
        error: function (err) {
            console.log(`failed to get a recommendation ticket:\n${err}`);
        }

    });
}

App.getBestTicket = function (targetElement, date) {
    $.get({
        url: `/${App.TableController}/BestOfDateTable`,
        data: {
            tableDate: date
        },
        success: function (result) {
            targetElement.append(result).fadeIn(300, "linear");
        },
        error: function (err) {
            console.log(`failed to get a sell ticket:\n${err}`);
        }

    });
}

App.toggleGraphSize = function (thisButton,canvasParent) {
    
    if (canvasParent.width() > App.SamllGraphWidth) {
        canvasParent.width(App.SamllGraphWidth);
        $(thisButton).html('&uarr;');
    } else {
        let newWidth = Math.round($(window).innerWidth() * 0.85);
        if (newWidth > App.SamllGraphWidth) {
            canvasParent.width(Math.round($(window).innerWidth() * 0.85));
            $(thisButton).html('&darr;');
        } else {
            canvasParent.width(App.SamllGraphWidth);
            $(thisButton).html('&uarr;');
        }
       
        
    }
   
}

App.UpateGraphSize = function (chartParent) {
    if (chartParent.width() > App.SamllGraphWidth) {
        chartParent.width(Math.round($(window).innerWidth()*0.85));
    }
    else {
        chartParent.width(App.SamllGraphWidth);
    }
}


App.updateOverviewSize = function (parent, chartParent) {
    if (parent.innerWidth() > App.overviewMaxWidth) {
        chartParent.width(App.overviewMaxWidth)
    }
    else {
        chartParent.width(parent.innerWidth());
    }
    
}


App.graphFactory = function (url,targetElement, data, createFunction) {
    $.get({
        url: `/${App.GraphController}/GetGraph`,
        success: function (content) {
            $.get({
                url: url,
                data: data,
                success: function (data) {
                    var elements = $(content);
                    var chr = elements.find('canvas');
                    $(window).resize(function () { App.UpateGraphSize(chr.parent()); });
                    targetElement.append(elements);
                    createFunction(chr[0], data);
                },
                error: function (err) {
                    console.log(`failed to get a graph data:\n${err}`);
                }

            });
        },
        error: function (err) {
            console.log(`failed to get a empty graph:\n${err}`);
        }
    });
}

App.getBankPriceGraph = function (targetElement, date, currency, isBuy) {
    App.graphFactory(`/${App.GraphController}/CurrencyPriceGraphData` ,targetElement, {
        graphDate: date,
        currency: currency,
        isBuy: isBuy
    }, App.createBankPriceGraph);
}
App.getPriceTimelineGraph = function (targetElement, startDate, endDate, currency, isBuy) {
    App.graphFactory(`/${App.GraphController}/CurrencyTimelineGraphData`, targetElement, {
        start: startDate,
        end: endDate,
        currency: currency,
        isBuy: isBuy
    }, App.createBankPriceTimelineGraph);
}

App.createBankPriceGraph = function (targetElement,graphData) {

    let datasets = [];
    
   
    let date = new Date(graphData.date);
    for ( let i = 0; i < graphData.bankNames.length; i++) {
        datasets.push(
            {
                label: graphData.bankNames[i],
                backgroundColor: App.RGBACOLORS[i],
                borderColor: App.COLORS[i],
                borderWidth: 1,
                data: [Math.round( graphData.bankValues[i]*10000)/10000]
            }
        );
    }
    let barChartData = {
        labels: [graphData.currency],
        datasets: datasets
    }

    let chartMin = Math.min(...graphData.bankValues);
    let chartMax = Math.max(...graphData.bankValues);
    let charfloor = chartMin - (chartMax - chartMin)*0.10;
    let chart = new Chart(targetElement, {
        type: 'bar',
        data: barChartData,
        options: {
            spanGaps: false,
            scales: {
                yAxes: [{
                    ticks: {
                        id: 'price',
                        min: charfloor,
                        type: 'linear',
                        stepSize: 0.1
                    }
                }]
            },
            responsive: true,
            responsiveAnimationDuration: 1000,
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: `${App.PrettyPrintDate(date)} ${graphData.currency} ${graphData.isBuy ? "buy" : "sell"} price `
            }
        }
    });
}

App.createBankPriceTimelineGraph = function (targetElement, graphData) {
    let startDate = new Date(graphData.start);
    let endDate = new Date(graphData.end);
    let datasets = [];
    let i = 0;
    for (let key in graphData.dataset) {
        datasets.push(
            {
                label: key,
                backgroundColor: App.RGBACOLORS[i],
                borderColor: App.COLORS[i],
                borderWidth: 1,
                data: [],
                fill: false
            }
        );
        i += 1;
    }

    let lineValues = []
    
    for (let label of graphData.labels) {
        let labelDate = new Date(label);
        let formatedDate = App.getFormatedDate(labelDate);
        let validValues = 0;
        let valueSum = 0;
        for (let dataset of datasets) {
            
            if (graphData.dataset[dataset.label][formatedDate] === undefined) {
                dataset.data.push(Number.NaN);
            }
            else {
                validValues += 1;
                valueSum += graphData.dataset[dataset.label][formatedDate];
                dataset.data.push(Math.round(graphData.dataset[dataset.label][formatedDate]*10000)/10000);
            }
        }
        if (validValues > 0) {
            let tmpVal = valueSum / validValues;
            lineValues.push(tmpVal);
        } 


        
    }
    //Fake regression
    let separator = Math.round(lineValues.length / 2)
    let startValues = lineValues.slice(0, separator);
    let endValues = lineValues.slice(separator, lineValues.length);

    let endValue = arrAvg(startValues);
    let value = arrAvg(endValues)

    endValue = Math.round(endValue * 10000) / 10000;
    value = Math.round(value * 10000) / 10000;


    //
    let n = lineValues.length;
    let xy = lineValues.map((value, index) => value * (index + 1));
    let x = lineValues;
    let y = lineValues.map((value, index) => index + 1);
    let x2 = lineValues.map(value => value * value);
    let xySum = arrSum(xy);
    let xSum = arrSum(x);
    let ySum = arrSum(y);
    let x2Sum = arrSum(x2);
    let aTmp = (xySum - xSum) / x2Sum;
    let b = ySum / (n + aTmp);
    let a = (xySum - (b * xSum)) / x2Sum;

    let yMin = arrMin(y);
    let yMax = arrMax(y);
    regValue = yMin * a + b;
    regEndValue = yMax * a + b;

    


    let labels = [];
    for (let j = 0; j < graphData.labels.length; j++) {
        labels.push(App.PrettyPrintDate(new Date(graphData.labels[j])));
    }


    let barChartData = {
        labels: labels,
        datasets: datasets
    }
    let chart = new Chart(targetElement, {
        type: 'line',
        data: barChartData,
        options: {
            spanGaps: true,
            scales: {
                yAxes: [{
                    ticks: {
                        id: 'price',
                        type: 'linear'
                    }
                }]
            },
            responsive: true,
            responsiveAnimationDuration: 1000,
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: ` ${graphData.currency} ${graphData.isBuy ? "buy" : "sell"} price from ${App.PrettyPrintDate(startDate)} to ${App.PrettyPrintDate(endDate)}`
            },
            annotation: {
                drawTime: 'beforeDatasetsDraw',
                events: ['click'],
                annotations: [{
                    id: 'hline',
                    type: 'line',
                    mode: 'horizontal',
                    scaleID: 'y-axis-0',
                    value: value,
                    endValue: endValue, 
                    borderColor: 'rgba(90, 90, 90, 0.6)',
                    borderWidth: 4
                }]
            }
        }
    });
}

App.chartColors = {
    red: 'rgb(255, 99, 132)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(75, 192, 192)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(201, 203, 207)'
};

App.color = function(index) {
    return COLORS[index % COLORS.length];
}

App.COLORS = [
    '#4dc9f6',
    '#f67019',
    '#f53794',
    '#537bc4',
    '#acc236',
    '#166a8f',
    '#00a950',
    '#58595b',
    '#8549ba'
];
App.RGBACOLORS = [
    'rgba(77, 201, 246, 0.5)',
    'rgba(246, 112, 25, 0.5)',
    'rgba(245, 55, 148, 0.5)',
    'rgba(83, 123, 196, 0.5)',
    'rgba(172, 194, 54, 0.5)',
    'rgba(22, 106, 143, 0.5)',
    'rgba(0, 169, 80, 0.5)',
    'rgba(88, 89, 91, 0.5)',
    'rgba(133, 73, 186, 0.5)'


]
 App.generateId = function () {
    // Math.random should be unique because of its seeding algorithm.
    // Convert it to base 36 (numbers + letters), and grab the first 9 characters
    // after the decimal.
    return '_' + Math.random().toString(36).substr(2, 9);
};
