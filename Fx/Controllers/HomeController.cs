using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fx.Models;
using System.Data;
using System.Data.Entity;
using System.Web.Script.Serialization;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Transactions;

namespace Fx.Controllers
{
    public class HomeController : Controller
    {
        private readonly fxEntities2 cn = new fxEntities2();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Zhucej()
        {
            return View();
        }
        public ActionResult Zhuce(jiao jiao)
        {
            var model = (from p in cn.jiao
                         where p.jlinq == jiao.jlinq
                         select p).ToList();
            var model2 = (from p in cn.jiao
                          where p.je == jiao.je
                          select p).ToList();
            if (model.Count > 0)
                return Content("该账户已被注册！");
            if (model2.Count > 0)
                return Content("该电子邮件已被注册！");
            if (ModelState.IsValid)
            {
                jiao.jid = 1;
                cn.jiao.Add(jiao);
                cn.SaveChanges();
                return Content("ok");
            }
            else
            {
                return Content("注册信息有误！");
            }
        }//教师注册
        public ActionResult Ling(string get, string id, string pwd)
        {
            if (get == "true")
            {
                var model = (from p in cn.jiao
                             where p.jlinq == id && p.jmima == pwd
                             select new Ling {
                                 id = p.jid.ToString(),
                                 name = p.jname
                             }).ToList();
                if (model.Count > 0)
                {
                    List<Ling> ls = new List<Ling>(model);
                    Session["jid"] = ls[0].id;
                    Session["name"] = ls[0].name;
                    return Content("ok");
                }
                else
                    return Content("账号或密码错误！");
            }
            if (get == "false")
            {
                var model = (from p in cn.bxue
                             where p.xid == id && p.mima == pwd
                             select new Ling
                             {
                                 id = p.xid,
                                 name = p.xname
                             }).ToList();
                if (model.Count > 0)
                {
                    List<Ling> ls = new List<Ling>(model);
                    Session["xid"] = ls[0].id;
                    Session["name"] = ls[0].name;
                    return Content("ok2");
                }
                else
                    return Content("账号或密码错误！");
            }
            return Content("未知错误！");
        }//登录
        public ActionResult Ban()
        {
            var se = (from p in cn.kechen
                      orderby p.kid ascending
                      select new Ling
                      {
                          id = p.kid.Trim(),
                          name = p.kname
                      }).Skip(3);
            if (Session["jid"] != null)
            {
                return View(se);
            }
            else
                return RedirectToAction("index");
        }
        public ActionResult Banxiang()
        {
            if (Session["jid"] != null)
            {
                int d = int.Parse(Session["jid"].ToString());
                var model = (from p in cn.ban
                             orderby p.bdate descending
                             where p.jid == d
                             select new banxiang
                             {
                                 id = p.bid.ToString(),
                                 name = p.bname,
                                 text = p.btext
                             }).ToList();
                return PartialView(model);
            }
            else
                return Content("数据读取失败，请重新登录！");
        }//班级信息
        public ActionResult Delban(string id)
        {
            var q = (from p in cn.ban
                     where p.bid.ToString() == id
                     select p).First();
            try
            {
                cn.ban.Remove(q);
                cn.SaveChanges();
            }
            catch
            {
                return Content("删除失败！");
            }
            int d = int.Parse(Session["jid"].ToString());
            var model = (from p in cn.ban
                         orderby p.bdate descending
                         where p.jid == d
                         select new banxiang
                         {
                             id = p.bid.ToString(),
                             name = p.bname,
                             text = p.btext
                         }).ToList();
            return PartialView("Banxiang", model);
        }
        public ActionResult Addban(ban ban)
        {
            int d = int.Parse(Session["jid"].ToString());
            if (ban.btext == null || ban.btext == "")
                ban.btext = "暂无介绍。。。";
            ban.jid = d;
            ban.k1 = "001";
            ban.k2 = "002";
            ban.k3 = "003";
            ban.bdate = DateTime.Now;
            if (ban.k4 == ban.k5 || ban.k4 == ban.k6 || ban.k5 == ban.k6)
                return Content("科目重复，请重新添加或刷新页面！");
            if (ModelState.IsValid)
            {
                cn.ban.Add(ban);
                cn.SaveChanges();
                var model = (from p in cn.ban
                             orderby p.bdate descending
                             where p.jid == d
                             select new banxiang
                             {
                                 id = p.bid.ToString(),
                                 name = p.bname,
                                 text = p.btext
                             }).ToList();

                return PartialView("Banxiang", model);
            }
            else
                return Content("信息错误，请重新添加或刷新页面");
        }
        public ActionResult Extban(Exitban ban)
        {
            int d = int.Parse(Session["jid"].ToString());
            if (ban.text == null || ban.text == "")
                ban.text = "暂无介绍。。。";
            var model = (from p in cn.ban
                         where p.bid == ban.id
                         select p).First();
            model.bname = ban.name;
            model.btext = ban.text;
            if (ModelState.IsValid)
            {
                cn.SaveChanges();
                //cn.Entry(ban).State = EntityState.Modified;
                var model2 = (from p in cn.ban
                              orderby p.bdate descending
                              where p.jid == d
                              select new banxiang
                              {
                                  id = p.bid.ToString(),
                                  name = p.bname,
                                  text = p.btext
                              }).ToList();

                return PartialView("Banxiang", model2);
            }
            return Content("信息有误，请重新刷新页面或重新添加！");
        }
        public ActionResult Main(int id)
        {
            if (Session["jid"] == null)
                return RedirectToAction("Index");
            Session["bid"] = id;
            return View();
        }//主页面
        public ActionResult Xue()
        {
            return PartialView();
        }
        public ActionResult Xuexiang()
        {
            int jid = int.Parse(Session["jid"].ToString());
            int bid = int.Parse(Session["bid"].ToString());
            var model = (from p in cn.bxue
                         join k in cn.ban on p.bid equals k.bid
                         where k.jid == jid && p.bid == bid
                         select new xuexiang
                         {
                             id = p.xid,
                             name = p.xname,
                             sex = p.xsex,
                             bir = p.xbir
                         }).ToList();
            return PartialView(model);
        }//学生信息
        public ActionResult Addxue(bxue xue)
        {
            int jid = int.Parse(Session["jid"].ToString());
            int bid = int.Parse(Session["bid"].ToString());
            xue.bid = bid;
            var x = (from p in cn.bxue
                     where p.xid == xue.xid
                     select new {
                         mima = p.mima
                     }).ToList();
            if (x.Count > 0)
                xue.mima = x[0].mima;
            else
                xue.mima = "111111";
            if (ModelState.IsValid)
            {
                try
                {
                    cn.bxue.Add(xue);
                    cn.SaveChanges();
                }
                catch
                {
                    return Content("添加失败，可能原因是学号重复！");
                }
            }
            else
                return Content("信息有误，请重新添加或刷新页面！");
            var model = (from p in cn.bxue
                         join k in cn.ban on p.bid equals k.bid
                         where k.jid == jid && p.bid == bid
                         select new xuexiang
                         {
                             id = p.xid,
                             name = p.xname,
                             sex = p.xsex,
                             bir = p.xbir
                         }).ToList();
            return PartialView("Xuexiang", model);
        }
        public ActionResult Delxue(string id)
        {
            int jid = int.Parse(Session["jid"].ToString());
            int bid = int.Parse(Session["bid"].ToString());
            var q = (from p in cn.bxue
                     where p.xid == id && p.bid == bid
                     select p).First();
            try
            {
                cn.bxue.Remove(q);
                cn.SaveChanges();
                var model = (from p in cn.bxue
                             join k in cn.ban on p.bid equals k.bid
                             where k.jid == jid && p.bid == bid
                             select new xuexiang
                             {
                                 id = p.xid,
                                 name = p.xname,
                                 sex = p.xsex,
                                 bir = p.xbir
                             }).ToList();
                return PartialView("Xuexiang", model);
            }
            catch
            {
                return Content("数据库错误，删除失败！");
            }
        }
        public ActionResult Exitxue(xuexiang xue)
        {
            int jid = int.Parse(Session["jid"].ToString());
            int bid = int.Parse(Session["bid"].ToString());
            var model = (from p in cn.bxue
                         where p.bid == bid && p.xid == xue.id
                         select p).First();
            model.xid = xue.id;
            model.xname = xue.name;
            model.xsex = xue.sex;
            model.xbir = xue.bir;
            if (ModelState.IsValid)
            {
                try
                {
                    cn.SaveChanges();
                    var model2 = (from p in cn.bxue
                                  join k in cn.ban on p.bid equals k.bid
                                  where k.jid == jid && p.bid == bid
                                  select new xuexiang
                                  {
                                      id = p.xid,
                                      name = p.xname,
                                      sex = p.xsex,
                                      bir = p.xbir
                                  }).ToList();
                    return PartialView("Xuexiang", model2);
                }
                catch
                {
                    return Content("数据库错误，修改失败！");
                }
            }
            else return Content("信息有误，请重新执行或刷新页面！");
        }
        public ActionResult Dao()
        {
            int bid = int.Parse(Session["bid"].ToString());
            try
            {
                var model = (from p in cn.bxue
                             where p.bid == bid
                             select new xuexiang()
                             {
                                 id = p.xid,
                                 name = p.xname,
                                 sex = p.xsex,
                                 bir = p.xbir
                             }).ToList();
                int conunt = model.Count;
                if (conunt > 0)
                {
                    XSSFWorkbook workbook = new XSSFWorkbook();
                    workbook.CreateSheet("学生信息");
                    XSSFSheet sheet = (XSSFSheet)workbook.GetSheet("学生信息");
                    for (int h = 0; h < conunt + 1; h++)
                        sheet.CreateRow(h);
                    XSSFRow sheetrow = (XSSFRow)sheet.GetRow(0);
                    XSSFCell[] sheetcell = new XSSFCell[4];
                    for (int i = 0; i < 4; i++)
                        sheetcell[i] = (XSSFCell)sheetrow.CreateCell(i);
                    sheetcell[0].SetCellValue("学号");
                    sheetcell[1].SetCellValue("姓名");
                    sheetcell[2].SetCellValue("性别");
                    sheet.SetColumnWidth(0, 14 * 256);
                    sheet.SetColumnWidth(3, 18 * 256);
                    sheetcell[3].SetCellValue("出生日期");
                    List<xuexiang> list = new List<xuexiang>(model);
                    for (int i = 0; i < conunt; i++)
                    {
                        sheetrow = (XSSFRow)sheet.GetRow(i + 1);
                        for (int c = 0; c < 4; c++)
                            sheetcell[c] = (XSSFCell)sheetrow.CreateCell(c);
                        sheetcell[0].SetCellValue(list[i].id);
                        sheetcell[1].SetCellValue(list[i].name);
                        sheetcell[2].SetCellValue(list[i].sex);
                        sheetcell[3].SetCellValue(list[i].bir.ToString("yyyy.MM.dd"));
                    }
                    if (!Directory.Exists("E:\\ds"))
                        Directory.CreateDirectory("E:\\ds");
                    FileStream fss = new FileStream(@"E:\ds\" + bid + "学生信息.xlsx", FileMode.Create);
                    workbook.Write(fss);
                    fss.Close();
                    workbook.Close();
                    return File(@"E:\ds\" + bid + "学生信息.xlsx", "application/vnd.ms-xlsx", "" + bid + "学生信息.xlsx");
                }
                else
                    return Content("该班级下暂无学生数据！");
            }
            catch
            {
                return Content("后台错误导出失败，请稍后再试！");
            }
        }//导出学生信息
        public ActionResult Execlin()
        {
            int jid = int.Parse(Session["jid"].ToString());
            int bid = int.Parse(Session["bid"].ToString());
            if (!Directory.Exists("E:\\dx"))
                Directory.CreateDirectory("E:\\dx");
            try
            {
                Request.Files["exls"].SaveAs("E:\\dx\\" + Request.Files["exls"].FileName);
                if (Request.Files["exls"].FileName.LastIndexOf(".xlsx") > 0)
                {
                    FileStream fileStream = new FileStream(@"E:\dx\" + Request.Files["exls"].FileName + "", FileMode.Open);
                    IWorkbook workbook = new XSSFWorkbook(fileStream);
                    ISheet sheet = workbook.GetSheetAt(0);
                    IRow row;
                    row = sheet.GetRow(0);
                    if (row.GetCell(0).ToString() != "学号")
                    {
                        System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                        return Content("上传的文件内容有误，请对比查看示例或下载模板进行导入！");
                    }
                    if (row.GetCell(1).ToString() != "姓名")
                    {
                        System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                        return Content("上传的文件内容有误，请对比查看示例或下载模板进行导入！");
                    }
                    if (row.GetCell(2).ToString() != "性别")
                    {
                        System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                        return Content("上传的文件内容有误，请对比查看示例或下载模板进行导入！");
                    }
                    if (row.GetCell(3).ToString() != "出生日期")
                    {
                        System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                        return Content("上传的文件内容有误，请对比查看示例或下载模板进行导入！");
                    }
                    int i = 1;
                    using (TransactionScope sc = new TransactionScope())
                    {
                        try
                        {
                            for (; i < sheet.LastRowNum + 1; i++)
                            {
                                row = sheet.GetRow(i);
                                int ip = row.LastCellNum;
                                if (row != null)
                                {
                                    bxue Student = new bxue();
                                    Student.xid = row.GetCell(0).ToString();
                                    Student.bid = bid;
                                    Student.xname = row.GetCell(1).ToString();
                                    Student.xsex = row.GetCell(2).ToString();
                                    string f = row.GetCell(3).ToString();
                                    var x = (from p in cn.bxue
                                             where p.xid == Student.xid
                                             select new
                                             {
                                                 mima = p.mima
                                             }).ToList();
                                    if (x.Count > 0)
                                        Student.mima = x[0].mima;
                                    else
                                        Student.mima = "111111";
                                    while (f.IndexOf(".") > 0)
                                    {
                                        f = f.Replace(".", "-");
                                    }
                                    Student.xbir = DateTime.Parse(DateTime.Parse(f).ToString("yyyy/MM/dd"));
                                    if (ModelState.IsValid)
                                    {
                                        cn.bxue.Add(Student);
                                        cn.SaveChanges();
                                    }
                                }
                            }
                            fileStream.Close();
                            workbook.Close();
                            sc.Complete();
                            System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                            return Content("导入成功，共" + (i - 1).ToString() + "条数据！");
                        }
                        catch (Exception ec)
                        {
                            fileStream.Close();
                            workbook.Close();
                            System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                            return Content("第" + i + "行数据有误，请对比示例更改！" + ec.Message);
                        }
                    }
                }
                else
                {
                    System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                    return Content("上传的文件格式有误，请下载模板进行导入！");
                }
            }
            catch
            {
                return Content("文件上传有误,请重新上传!");
            }
        }//导入
        public ActionResult Fen()
        {
            int bid = int.Parse(Session["bid"].ToString());
            selban_Result kemu = cn.selban(bid).First();
            return PartialView(kemu);
        }
        public ActionResult Chenx()
        {
            int bid = int.Parse(Session["bid"].ToString());
            var model = (from p in cn.bxue
                         join k in cn.bchen
                         on new {p.bid,p.xid } equals new {k.bid,k.xid } 
                         where k.bid == bid
                         select new Chenx
                         {
                             id = k.xid,
                             name = p.xname,
                             k1 = k.c1,
                             k2 = k.c2,
                             k3 = k.c3,
                             k4 = k.c4,
                             k5 = k.c5,
                             k6 = k.c6
                         }).ToList();
            return PartialView(model);
        }//成绩详情
        public ActionResult Addfen(bchen chen)
        {
            int bid = int.Parse(Session["bid"].ToString());
            chen.bid = bid;
            try {
                cn.bchen.Add(chen);
                cn.SaveChanges();
                var model = (from p in cn.bxue
                             join k in cn.bchen
                             on new { p.bid, p.xid } equals new { k.bid, k.xid }
                             where k.bid == bid
                             select new Chenx
                             {
                                 id = k.xid,
                                 name = p.xname,
                                 k1 = k.c1,
                                 k2 = k.c2,
                                 k3 = k.c3,
                                 k4 = k.c4,
                                 k5 = k.c5,
                                 k6 = k.c6
                             }).ToList();
                return PartialView("Chenx", model);
            }
            catch {
                return Content("添加失败！请重试或刷新页面！");
            }
        }
        public ActionResult Exitfue(bchen chen)
        {
            int bid = int.Parse(Session["bid"].ToString());
            chen.bid = bid;
            if (ModelState.IsValid)
            {
                cn.Entry(chen).State = EntityState.Modified;
                cn.SaveChanges();
                var model = (from p in cn.bxue
                             join k in cn.bchen
                             on new { p.bid, p.xid } equals new { k.bid, k.xid }
                             where k.bid == bid
                             select new Chenx
                             {
                                 id = k.xid,
                                 name = p.xname,
                                 k1 = k.c1,
                                 k2 = k.c2,
                                 k3 = k.c3,
                                 k4 = k.c4,
                                 k5 = k.c5,
                                 k6 = k.c6
                             }).ToList();
                return PartialView("Chenx", model);
            }
            else
                return Content("修改失败！请重试或刷新页面！");
        }
        public ActionResult Delfen(string id)
        {
            int jid = int.Parse(Session["jid"].ToString());
            int bid = int.Parse(Session["bid"].ToString());
            var q = (from p in cn.bchen
                     where p.xid == id && p.bid == bid
                     select p).First();
            try
            {
                cn.bchen.Remove(q);
                cn.SaveChanges();
                var model = (from p in cn.bxue
                             join k in cn.bchen
                             on new { p.bid, p.xid } equals new { k.bid, k.xid }
                             where k.bid == bid
                             select new Chenx
                             {
                                 id = k.xid,
                                 name = p.xname,
                                 k1 = k.c1,
                                 k2 = k.c2,
                                 k3 = k.c3,
                                 k4 = k.c4,
                                 k5 = k.c5,
                                 k6 = k.c6
                             }).ToList();
                return PartialView("Chenx", model);
            }
            catch
            {
                return Content("数据库错误，删除失败！");
            }
        }
        public ActionResult Daox()
        {
            int bid = int.Parse(Session["bid"].ToString());
            try
            {
                var model = (from p in cn.bxue
                             join k in cn.bchen
                             on new { p.bid, p.xid } equals new { k.bid, k.xid }
                             where k.bid == bid
                             select new Chenx
                             {
                                 id = k.xid,
                                 name = p.xname,
                                 k1 = k.c1,
                                 k2 = k.c2,
                                 k3 = k.c3,
                                 k4 = k.c4,
                                 k5 = k.c5,
                                 k6 = k.c6
                             }).ToList();
                int conunt = model.Count;
                selban_Result kemu = cn.selban(bid).First();
                if (conunt > 0)
                {
                    XSSFWorkbook workbook = new XSSFWorkbook();
                    workbook.CreateSheet("成绩信息");
                    XSSFSheet sheet = (XSSFSheet)workbook.GetSheet("成绩信息");
                    for (int h = 0; h < conunt + 1; h++)
                        sheet.CreateRow(h);
                    XSSFRow sheetrow = (XSSFRow)sheet.GetRow(0);
                    XSSFCell[] sheetcell = new XSSFCell[8];
                    for (int i = 0; i < 8; i++)
                        sheetcell[i] = (XSSFCell)sheetrow.CreateCell(i);
                    sheetcell[0].SetCellValue("学号");
                    sheetcell[1].SetCellValue("姓名");
                    sheet.SetColumnWidth(0, 14 * 256);
                    sheet.SetColumnWidth(1, 18 * 256);
                    sheetcell[2].SetCellValue(kemu.c1);
                    sheetcell[3].SetCellValue(kemu.c2);
                    sheetcell[4].SetCellValue(kemu.c3);
                    sheetcell[5].SetCellValue(kemu.c4);
                    sheetcell[6].SetCellValue(kemu.c5);
                    sheetcell[7].SetCellValue(kemu.c6);
                    List<Chenx> list = new List<Chenx>(model);
                    for (int i = 0; i < conunt; i++)
                    {
                        sheetrow = (XSSFRow)sheet.GetRow(i + 1);
                        for (int c = 0; c < 8; c++)
                            sheetcell[c] = (XSSFCell)sheetrow.CreateCell(c);
                        sheetcell[0].SetCellValue(list[i].id);
                        sheetcell[1].SetCellValue(list[i].name);
                        sheetcell[2].SetCellValue(list[i].k1.ToString());
                        sheetcell[3].SetCellValue(list[i].k2.ToString());
                        sheetcell[4].SetCellValue(list[i].k3.ToString());
                        sheetcell[5].SetCellValue(list[i].k4.ToString());
                        sheetcell[6].SetCellValue(list[i].k5.ToString());
                        sheetcell[7].SetCellValue(list[i].k6.ToString());
                    }
                    if (!Directory.Exists("E:\\ds"))
                        Directory.CreateDirectory("E:\\ds");
                    FileStream fss = new FileStream(@"E:\ds\" + bid + "成绩信息.xlsx", FileMode.Create);
                    workbook.Write(fss);
                    fss.Close();
                    workbook.Close();
                    return File(@"E:\ds\" + bid + "成绩信息.xlsx", "application/vnd.ms-xlsx", "" + bid + "成绩信息.xlsx");
                }
                else
                    return Content("该班级下暂无学生数据！");
            }
            catch
            {
                return Content("后台错误导出失败，请稍后再试！");
            }
        }//导出成绩
        public ActionResult Rux()
        {
            int jid = int.Parse(Session["jid"].ToString());
            int bid = int.Parse(Session["bid"].ToString());
            selban_Result kemu = cn.selban(bid).First();
            if (!Directory.Exists("E:\\dx"))
                Directory.CreateDirectory("E:\\dx");
            try
            {
                Request.Files["exls"].SaveAs("E:\\dx\\" + Request.Files["exls"].FileName);
                if (Request.Files["exls"].FileName.LastIndexOf(".xlsx") > 0)
                {
                    FileStream fileStream = new FileStream(@"E:\dx\" + Request.Files["exls"].FileName + "", FileMode.Open);
                    IWorkbook workbook = new XSSFWorkbook(fileStream);
                    ISheet sheet = workbook.GetSheetAt(0);
                    IRow row;
                    row = sheet.GetRow(0);
                    if (row.GetCell(0).ToString() != "学号")
                    {
                        System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                        return Content("上传的文件内容有误，请对比查看示例或下载模板进行导入！");
                    }
                    if (row.GetCell(1).ToString() != "姓名")
                    {
                        System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                        return Content("上传的文件内容有误，请对比查看示例或下载模板进行导入！");
                    }
                    if (row.GetCell(2).ToString() != kemu.c1)
                    {
                        System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                        return Content("上传的文件内容有误，请对比查看示例或下载模板进行导入！");
                    }
                    if (row.GetCell(3).ToString() != kemu.c2)
                    {
                        System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                        return Content("上传的文件内容有误，请对比查看示例或下载模板进行导入！");
                    }
                    if (row.GetCell(4).ToString() != kemu.c3)
                    {
                        System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                        return Content("上传的文件内容有误，请对比查看示例或下载模板进行导入！");
                    }
                    if (row.GetCell(5).ToString() != kemu.c4)
                    {
                        System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                        return Content("上传的文件内容有误，请对比查看示例或下载模板进行导入！");
                    }
                    if (row.GetCell(6).ToString() != kemu.c5)
                    {
                        System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                        return Content("上传的文件内容有误，请对比查看示例或下载模板进行导入！");
                    }
                    if (row.GetCell(7).ToString() != kemu.c6)
                    {
                        System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                        return Content("上传的文件内容有误，请对比查看示例或下载模板进行导入！");
                    }
                    int i = 1;
                    using (TransactionScope sc = new TransactionScope())
                    {
                        try
                        {
                            for (; i < sheet.LastRowNum + 1; i++)
                            {
                                row = sheet.GetRow(i);
                                int ip = row.LastCellNum;
                                if (row != null)
                                {
                                    bchen Student = new bchen();
                                    Student.xid = row.GetCell(0).ToString();
                                    Student.bid = bid;
                                    Student.c1 = decimal.Parse(row.GetCell(2).ToString());
                                    Student.c2 = decimal.Parse(row.GetCell(3).ToString());
                                    Student.c3 = decimal.Parse(row.GetCell(4).ToString());
                                    Student.c4 = decimal.Parse(row.GetCell(5).ToString());
                                    Student.c5 = decimal.Parse(row.GetCell(6).ToString());
                                    Student.c6 = decimal.Parse(row.GetCell(7).ToString());
                                    if (ModelState.IsValid)
                                    {
                                        cn.bchen.Add(Student);
                                        cn.SaveChanges();
                                    }
                                }
                            }
                            fileStream.Close();
                            workbook.Close();
                            sc.Complete();
                            System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                            return Content("导入成功，共" + (i - 1).ToString() + "条数据！");
                        }
                        catch (Exception ec)
                        {
                            fileStream.Close();
                            workbook.Close();
                            System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                            return Content("第" + i + "行数据有误，请对比示例更改！" + ec.Message);
                        }
                    }
                }
                else
                {
                    System.IO.File.Delete("E:\\dx\\" + Request.Files["exls"].FileName);
                    return Content("上传的文件格式有误，请下载模板进行导入！");
                }
            }
            catch
            {
                return Content("文件上传有误,请重新上传!");
            }
        }//导入
        public ActionResult Dmu()
        {
            int bid = int.Parse(Session["bid"].ToString());
            try
            {
                var model = (from p in cn.bxue
                             where p.bid == bid
                             select new Chenx
                             {
                                 id = p.xid,
                                 name = p.xname,
                             }).ToList();
                int conunt = model.Count;
                selban_Result kemu = cn.selban(bid).First();
                if (conunt > 0)
                {
                    XSSFWorkbook workbook = new XSSFWorkbook();
                    workbook.CreateSheet("成绩模板");
                    XSSFSheet sheet = (XSSFSheet)workbook.GetSheet("成绩模板");
                    for (int h = 0; h < conunt + 1; h++)
                        sheet.CreateRow(h);
                    XSSFRow sheetrow = (XSSFRow)sheet.GetRow(0);
                    XSSFCell[] sheetcell = new XSSFCell[8];
                    for (int i = 0; i < 8; i++)
                        sheetcell[i] = (XSSFCell)sheetrow.CreateCell(i);
                    sheetcell[0].SetCellValue("学号");
                    sheetcell[1].SetCellValue("姓名");
                    sheet.SetColumnWidth(0, 14 * 256);
                    sheet.SetColumnWidth(1, 18 * 256);
                    sheetcell[2].SetCellValue(kemu.c1);
                    sheetcell[3].SetCellValue(kemu.c2);
                    sheetcell[4].SetCellValue(kemu.c3);
                    sheetcell[5].SetCellValue(kemu.c4);
                    sheetcell[6].SetCellValue(kemu.c5);
                    sheetcell[7].SetCellValue(kemu.c6);
                    List<Chenx> list = new List<Chenx>(model);
                    for (int i = 0; i < conunt; i++)
                    {
                        sheetrow = (XSSFRow)sheet.GetRow(i + 1);
                        for (int c = 0; c < 8; c++)
                            sheetcell[c] = (XSSFCell)sheetrow.CreateCell(c);
                        sheetcell[0].SetCellValue(list[i].id);
                        sheetcell[1].SetCellValue(list[i].name);
                        sheetcell[2].SetCellValue("");
                        sheetcell[3].SetCellValue("");
                        sheetcell[4].SetCellValue("");
                        sheetcell[5].SetCellValue("");
                        sheetcell[6].SetCellValue("");
                        sheetcell[7].SetCellValue("");
                    }
                    if (!Directory.Exists("E:\\ds"))
                        Directory.CreateDirectory("E:\\ds");
                    FileStream fss = new FileStream(@"E:\ds\" + bid + "成绩模板.xlsx", FileMode.Create);
                    workbook.Write(fss);
                    fss.Close();
                    workbook.Close();
                    return File(@"E:\ds\" + bid + "成绩模板.xlsx", "application/vnd.ms-xlsx", "" + bid + "成绩模板.xlsx");
                }
                else
                    return Content("该班级下暂无学生数据！");
            }
            catch
            {
                return Content("后台错误导出失败，请稍后再试！");
            }
        }//下载模板
        public ActionResult Gen()
        {
            if (Session["jid"] == null)
                return RedirectToAction("/Index");
            int jid = int.Parse(Session["jid"].ToString());
            jiao j = cn.jiao.Find(jid);
            return PartialView(j);
        }
        [HttpPost]
        public ActionResult Gen(jiao j)
        {
            int jid = int.Parse(Session["jid"].ToString());
            j.jid = jid;
            var model = (from p in cn.jiao
                         where p.jid== jid
                         select new
                         {
                             jlinq=p.jlinq,
                             je=p.je,
                             mima=p.jmima
                         }).ToList();
            if (j.jmima == "" || j.jmima == null)
                j.jmima = model[0].mima;
            j.jlinq = model[0].jlinq;
            if (j.je != model[0].je) 
            {
                var model2 = (from p in cn.jiao
                              where p.je == j.je
                              select p).ToList();
                if (model2.Count > 0)
                    return Content("该电子邮件已被注册！");
            }
            if (ModelState.IsValid)
            {
                cn.Entry(j).State = EntityState.Modified;
                cn.SaveChanges();
                Session.Clear();
                return Content("ok");
            }
            else
            {
                return Content("信息有误！");
            }
        }
        public ActionResult Fenmain()
        {
            if (Session["bid"] == null)
            {
                RedirectToAction("/Index");
            }
            return PartialView();
        }
        public ActionResult Jibu1()
        {
            int bid = int.Parse(Session["bid"].ToString());
            var model = (from p in cn.bxinx(bid)
                         select p).ToList();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string s = js.Serialize(model);
            return Content(s);
        }//以下为分析方法
        public ActionResult Jibu2()
        {
            int bid = int.Parse(Session["bid"].ToString());
            var model = (from p in cn.fenshu(bid)
                         select new
                         {
                             avg = p.avg,
                             max = p.max,
                             min = p.min,
                             zhong = p.zhong
                         }).ToList();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string s = js.Serialize(model);
            return Content(s);
        }
        public ActionResult Jibu3()
        {
            int bid = int.Parse(Session["bid"].ToString());
            var model = (from p in cn.zhan(bid)
                         select new
                         {
                             one = p.one,
                             two = p.two,
                             three = p.three,
                             four = p.four,
                             cot = p.cot
                         }).ToList();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string s = js.Serialize(model);
            return Content(s);
        }
        public ActionResult Kufen()
        {
            int bid = int.Parse(Session["bid"].ToString());
            var model = (from p in cn.selban(bid)
                         select p).ToList();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string s = js.Serialize(model);
            var model2 = (from p in cn.kemu(bid)
                          select p).ToList();
            string q = js.Serialize(model2);
            q = "{\"kname\":" + s + ",\"kfen\":" + q + "}";
            return Content(q);
        }
        public ActionResult Zhanbi()
        {
            int bid = int.Parse(Session["bid"].ToString());
            var model = (from p in cn.zhan(bid)
                         select p).ToList();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string s = js.Serialize(model);
            return Content(s);
        }
        public ActionResult Top()
        {
            int bid = int.Parse(Session["bid"].ToString());
            var model = (from p in cn.fentop(bid)
                         select new
                         {
                             fen = p.su,
                             name = p.bname
                         }).ToList();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string s = js.Serialize(model);
            return Content(s);
        }
        public ActionResult Bandui()
        {
            int bid = int.Parse(Session["bid"].ToString());
            var model = (from p in cn.bandb(bid)
                         select new bandb_Result
                         {
                             jname = p.jname,
                             bname = p.bname,
                             pin = p.pin,
                             zd = p.zd,
                             zx = p.zx
                         }).ToList();
            return PartialView(model);
        }
        public ActionResult Pindui()
        {
            int bid = int.Parse(Session["bid"].ToString());
            var model = (from p in cn.bandb(bid)
                         select new
                         {
                             bname = p.bname,
                             pin = p.pin,
                         }).ToList();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string s = js.Serialize(model);
            return Content(s);
        }
    }
}