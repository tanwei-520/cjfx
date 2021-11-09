function Jibu1() {
    $.ajax({
        dataType: "json",
        type: "get",
        url: "/Home/Jibu1",
        data: {},
        async: true,
        success: function (data) {
            $("#biaoti").text(data[0]+"成绩分析");
            $("#bxinxi").children('th').eq(0).text(data[0]);
            $("#bxinxi").children('th').eq(1).text(data[1]);
            $("#bxinxi").children('th').eq(2).text(data[2]);
            $("#bxinxi").children('th').eq(3).text(data[3]);
        },
        error: function () {
            $("#tan").show();
            $("#ti").text("信息1加载失败，请重试");
        }
    })
}
function Jibu2() {
    $.ajax({
        dataType: 'json',
        type: "get",
        url: "/Home/Jibu2",
        data: {},
        async: true,
        success: function (data) {
            if ($("#bxinxi").children('th').eq(3).text()!=0) {
                $("#zongfen").children('th').eq(0).text(data[0].avg);
                $("#zongfen").children('th').eq(1).text(data[0].zhong);
                $("#zongfen").children('th').eq(2).text(data[0].max);
                $("#zongfen").children('th').eq(3).text(data[0].min);
            }
        },
        error: function () {
            $("#tan").show();
            $("#ti").text("信息2加载失败，请重试");
        }
    })
}
function Jibu3() {
    $.ajax({
        dataType: 'json',
        type: "get",
        url: "/Home/Jibu3",
        data: {},
        async: true,
        success: function (data) {
            if ($("#bxinxi").children('th').eq(3).text() != 0) {
                $("#lv").children('th').eq(0).text(((data[0].one / data[0].cot) * 100).toFixed(2) + "%");
                $("#lv").children('th').eq(1).text(((data[0].two / data[0].cot) * 100).toFixed(2) + "%");
                $("#lv").children('th').eq(2).text(((data[0].three / data[0].cot) * 100).toFixed(2) + "%");
                $("#lv").children('th').eq(3).text(((data[0].four / data[0].cot) * 100).toFixed(2) + "%");
            }
        },
        error: function () {
            $("#tan").show();
            $("#ti").text("信息3加载失败，请重试");
        }
    })
}
function Kufen() {
    $("#kefen").removeAttr("_echarts_instance_").empty();
    var myChart = echarts.init(document.getElementById('kefen'));
    $.ajax({
        dataType: 'json',
        type: "get",
        url: "/Home/Kufen",
        data: {},
        async: true,
        success: function (data) {
            if ($("#bxinxi").children('th').eq(3).text() != 0) {
                $("#ketext").text("此次考试" + data.kname[0].c1 + "平均分：" + data.kfen[0].c1 + "，" + data.kname[0].c2 + "平均分：" + data.kfen[0].c2 + "，" + data.kname[0].c3 + "平均分：" + data.kfen[0].c3 + "，" + data.kname[0].c4 + "平均分：" + data.kfen[0].c4 + "，" + data.kname[0].c5 + "平均分：" + data.kfen[0].c5 + "，" + data.kname[0].c6 + "平均分：" + data.kfen[0].c6 + "。");
            }
            else
                $("#ketext").text("暂无数据！");
            var option = {
                title: {
                    text: '科目能力分析',
                },
                tooltip: {},
                radar: {
                    // shape: 'circle',
                    name: {
                        textStyle: {
                            color: '#333333',
                            backgroundColor: '#999',
                            borderRadius: 3,
                            padding: [3, 5]
                        }
                    },
                    indicator: [
                        { name: data.kname[0].c1+'', max: 150 },
                        { name: data.kname[0].c2 + '', max: 150 },
                        { name: data.kname[0].c3 + '', max: 150 },
                        { name: data.kname[0].c4 + '', max: 100 },
                        { name: data.kname[0].c5 + '', max: 100 },
                        { name: data.kname[0].c6 + '', max: 100 }
                    ]
                },
                series: [{
                    name: '科目能力分析',
                    type: 'radar',
                    // areaStyle: {normal: {}},
                    data: [
                        {
                            value: [data.kfen[0].c1, data.kfen[0].c2, data.kfen[0].c3, data.kfen[0].c4, data.kfen[0].c5, data.kfen[0].c6],
                        }
                    ]
                }]
            };
            myChart.setOption(option, true);
        },
        error: function () {
            $("#tan").show();
            $("#ti").text("信息4加载失败，请重试");
        }
    })
}
function Zhanbi() {
    $("#zhanbi1").removeAttr("_echarts_instance_").empty();
    var myChart = echarts.init(document.getElementById('zhanbi1'));
    $("#zhanbi2").removeAttr("_echarts_instance_").empty();
    var myChart2 = echarts.init(document.getElementById('zhanbi2'));
    $.ajax({
        dataType: 'json',
        type: "get",
        url: "/Home/Zhanbi",
        data: {},
        async: true,
        success: function (data) {
            var option = {
                title: {
                    text: "分数段人数统计"
                },
                tooltip: {},
                xAxis: {
                    data:["不及格","及格","良好","优秀"]
                },
                yAxis: {},
                series: [{
                    name: '人数',
                    type: 'bar',
                    data: [data[0].one, data[0].two, data[0].three, data[0].four],
                    label: {
                        normal: {
                            position: 'top',
                            show: true
                        }
                    },
                }]
            };
            myChart.setOption(option, true);
            var option2 = {
                tooltip: {
                    trigger: 'item',
                    formatter: '{a} <br/>{b}: {c} ({d}%)'
                },
                title: {
                    text: '分数段人数占比',
                },
                series: [
                    {
                        name: '分数段人数占比',
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', '60%'],
                        avoidLabelOverlap: false,
                        label: {
                            show: true,
                            position: 'left'
                        },
                        emphasis: {
                            label: {
                                show: true,
                                fontSize: '30',
                                fontWeight: 'bold'
                            }
                        },
                        labelLine: {
                            show: true
                        },
                        data: [{ value: data[0].one == 0 ? null : data[0].one, name: "不及格" }, { value: data[0].two == 0 ? null : data[0].two, name: "及格" }, { value: data[0].three == 0 ? null : data[0].three, name: "良好" }, { value: data[0].four == 0 ? null : data[0].four, name: "优秀" },]
                    }
                ]
            };
            myChart2.setOption(option2, true);
        },
        error: function () {
            $("#tan").show();
            $("#ti").text("信息5加载失败，请重试");
        }
    })
}
function Top() {
    $.ajax({
        dataType: 'json',
        type: "get",
        url: "/Home/Top",
        data: {},
        async: true,
        success: function (data) {
            if ($("#bxinxi").children('th').eq(3).text() != 0) {
                $("#zhanbi3").text("此次考试总分前五名为：" + data[0].name + " " + data[0].fen + "," + data[1].name + " " + data[1].fen + "," + data[2].name + " " + data[2].fen + "," + data[3].name + " " + data[3].fen + "," + data[4].name + " " + data[4].fen + ",最后五名为：" + data[5].name + " " + data[5].fen + "," + data[6].name + " " + data[6].fen + "," + data[7].name + " " + data[7].fen + "," + data[8].name + " " + data[8].fen + "," + data[9].name + " " + data[9].fen + "。");
            }
            else
                $("#zhanbi3").text("暂无数据！");
        },
        error: function () {
            $("#tan").show();
            $("#ti").text("信息6加载失败，请重试");
        }
    })
}
function Pindui() {
    $("#Pindui").removeAttr("_echarts_instance_").empty();
    var myChart = echarts.init(document.getElementById('Pindui'));
    $.ajax({
        dataType: 'json',
        type: "get",
        url: "/Home/Pindui",
        data: {},
        async: true,
        success: function (data) {
            var x=[];
            var y=[];
            for (var i = 0; i < data.length; i++) {
                x[i] = data[i].bname;
                y[i] = data[i].pin;
            }
            if (data.length > 6 && data.length <= 12)
                $("#Pindui canvas").height(600);
            else if (data.length > 12)
                $("#Pindui canvas").height(800);
            var option = {
                tooltip: {},
                xAxis: {
                    type: 'value',
                },
                yAxis: {
                    type: 'category',
                    data:x
                },
                series: [{
                    name: '平均分',
                    type: 'bar',
                    data: y,
                    label: {
                        normal: {
                            position: 'right',
                            show: true
                        }
                    },
                }]
            };
            myChart.setOption(option, true);
        },
        error: function () {
            $("#tan").show();
            $("#ti").text("信息7加载失败，请重试");
        }
    })
}