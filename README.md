# UnityDemo

## SpaceDefense


    概述：
        SpaceDefense是一款简单的TD小游戏，玩家通过消耗不同金币创造对应的防守单位，自动攻击杀死每一波附近的怪物。
		    每杀死一只怪物会得到一定金币奖励。
		    怪物会从出生点一直逃向终点，如有一个幸存怪物成功逃脱至终点，玩家生命-1，玩家生命为0是，游戏失败。
		    玩家如果成功守住所有波数的怪物并且生命大于0，则游戏成功通关。
        
        
操作：
    鼠标点击游戏右上角的头像（防守单位），点击不放，再拖动鼠标到地图上的绿色空地，
    松开鼠标成功创造一个对应的防守单位，防守单位会自动攻击附近的怪物。
    如果金币不够则不会创造防守单位，并会提示金币不足。
    金币获取来源，杀死怪物获得。
    鼠标拖拽屏幕可移动视野。
    
    

该游戏会根据配置文件（GameConfigEdit.xml）进行游戏数据配置，玩家可自行修改配置文件游玩。

    配置文件路径：C:\Users\电脑名字\AppData\LocalLow\justguang\SpaceDefense\GameConfigEdit.xml
    【Android端的配置文件在 Android\data\com.justguang.SpaceDefense\files\GameConfigEdit.xml】

ps:默认配置文件不存在，需要游玩一次游戏，程序自动生成默认文件
	
	
******************************************************************************************************

### [SpaceDefense_v1.2](https://github.com/justguang/UnityDemo/releases/tag/SpaceDefense_v1.2)

对比 v1.0 ：
    1. 调整消息提示的UI位置 
    2. 增加视野边界（相机移动边界限制） 

*******************************************************************************************************

### [SpaceDefense_v1.0](https://github.com/justguang/UnityDemo/releases/tag/SpaceDefense_v1.0)

*******************************************************************************************************

#### 游戏截图：

<img src="https://img2020.cnblogs.com/blog/2518177/202110/2518177-20211028232344010-529876749.png" style="zoom:50%">

<img src="https://img2020.cnblogs.com/blog/2518177/202110/2518177-20211029102644566-657552130.jpg" style="zoom:50%">




