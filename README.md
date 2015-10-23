# Common
C#常用类汇总

本项目高度集中汇聚开发中常用的一些通用类，比如SQL帮助类，ini文件操作类等等。
详细内容如下：
1，iniFile.cs  add by  zhangbc 2015-07-09
该类主要实现方法有：
获取所有的结点名称；
获取某个结点下的所有的Key和Value；
写文件操作；
读取ini文件的所有的section结点；
读取ini文件下指定结点下Key值列表；
读取ini文件下指定key值的value（含重载）；
读取ini文件下指定结点的所有条目(key=value)；
创建空文件ini。

2,Fields.cs add by zhangbc 2015-07-09
应用单例模式实现，实现iniFile的各种操作，用途有：保存系统基础配置信息，保存用户登录信息，填充Property等。

3,propertyList.cs add by zhangbc 2015-07-09
实现自定义属性类

4，txtImport.cs  add by zhangbc 2015-10-23 
 	应用单例模式实现，将txt文本数据数据导入到DataGridView中.文本数据用制表符\t作为分隔符，第一行为表头，数据行从第二行开始，getTabHeader此方法是读取APP配置文件方法，为初始化表头信息的第二种方法。Appconfiguration如下：
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <!--表头配置 '_'作为分隔符使用-->
  <appSettings>
    <add key = "FinalSum_01" value = "提交金额"/>
    <add key = "FinalSum_02" value = "实发金额"/>
    …… ……
  </appSettings>
  
</configuration>

