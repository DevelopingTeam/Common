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
