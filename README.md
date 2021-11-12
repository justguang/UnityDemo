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
	
	

*****************************************************************************************************
### [SpaceDefense_v2.0](https://github.com/justguang/UnityDemo/releases/tag/SpaceDefense_v2.0)

对比上一版本 v1.2:
     1. 新增关卡概念（目前最多设置了3个关卡），玩家通关已解锁的最高关卡才能解锁下一个关卡。 
     2. 新增主界面，打开游戏首先进入的是主界面，点击不同关卡按钮进入不同关卡游戏。
     3. 新增菜单模块，提供继续游戏，重新游戏，返回主界面，退出游戏功能。
     4. 新增场景加载效果，场景之间切换显示场景加载界面，目前场景中数据较少所以加载效果不明显。
     5. 新增退出游戏前询问功能，PC端Esc键退出，Android端返回键退出。
     6. 修改配置文件，目前只配置了3个关卡数据，敌人波数修改，每波固定敌人数量，奖励调低。
     7. 修改android端的签名。

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




