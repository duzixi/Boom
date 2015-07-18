using UnityEngine;
using System.Collections;
/// <summary>
/// 脚本功能：扫雷游戏
/// 添加对象：Main Camera
/// 创建时间：2015年7月16日
/// 知识要点：
/// 1. 游戏算法：深搜、广搜、递归
/// 2. 委托与Lambda表达式
/// ----------------------
/// 
/// </summary>
public class Grid {
	public int number;   // 格子上的数字（周边雷数）
	public bool opened;  // 是否打开
	public bool hasBoom; // 是否有雷
}

public class Boom : MonoBehaviour {
	// 雷区设置
	public int row = 10; // 行数
	public int col = 10; // 列数
	public int boomNumber = 10; // 布雷次数
	private Grid[,] map; // 雷区地图
	public int size = 50; // 格子大小

	// 对(x,y)处格子的某种操作行为
	delegate void Do(int x, int y); 

	// 搜索(m,n)周边8个格子
	void SearchAround(int m, int n, Do d) {
		for (int i = -1; i <= 1; i++) {
			for (int j = -1; j <= 1; j++) {
				if (i != 0 || j != 0) { // 排除中心点
					int x = m + i;
					int y = n + j;
					if (x >= 0 && x < row &&
					    y >= 0 && y < col) {
						d(x, y);
					}
				}
			}
		}
	}
	
	void Start () {
		// 初始化雷区
		map = new Grid[row, col];
		for (int i = 0; i < row; i++) {
			for (int j = 0; j < col; j++) {
				map[i, j] = new Grid();
			}
		}
		// 布雷
		for (int i = 0; i < boomNumber; i++) {
			int x = Random.Range(0, row);
			int y = Random.Range(0, col);
			if (map[x, y].hasBoom) {
				i--; // 如果已经有雷，重复计算
			} else {
				map[x, y].hasBoom = true;
			}
		}
		// 计算格子数
		ComputeNum1();
	}
	// 方案一: 遍历所有格子，如果有雷，对周边格子数累加1
	void ComputeNum1(){
		for (int i = 0; i < row; i++) {
			for (int j = 0; j < col; j++) {
				if (map[i, j].hasBoom) {
					// Lambda表达式
					SearchAround(i, j, (x, y) => {
						map[x, y].number++;
					});
				}
			}
		}
	}
	
	// 方案二：遍历所有格子，数周边的雷数
	void ComputeNum2(){
		for (int i = 0; i < row; i++) {
			for (int j = 0; j < col; j++) {
				int count = 0;
				// Lambda表达式
				SearchAround(i, j, (x, y) => {
					if (map[x,y].hasBoom) {
						count++;
					}
				});
				map[i, j].number = count;
			}
		}
	}

	void Open(int m, int n) {
		map[m, n].opened = true; // 标记该位置已打开
		if (map[m, n].number == 0) { // 如果为空，继续展开周边格子
			SearchAround(m, n, (x, y) => {
				if (map[x, y].opened == false) {
					Open(x, y); // 递归调用，实现搜索
				}
			});
		}
	}

	// 展现层
	void OnGUI() {
		// 平铺扫雷格子
		for (int i = 0; i < row; i++) {
			for (int j = 0; j < col; j++) {
				// 显示格子中的信息，用于调试
				string tip = i + "," + j;
				tip += "\n" + map[i, j].number;
				// 用按钮背景颜色区分格子
				if (map[i, j].hasBoom) { // 有雷：红色
					GUI.backgroundColor = Color.red;
				} else {
					GUI.backgroundColor = Color.yellow;
				}
				Rect rect = new Rect(j * size, i * size, size, size);
				if (map[i, j].opened) {
					// Box表示已经开启过的格子
					GUI.Box (rect , tip);
				} else if (GUI.Button(rect, tip)){
					// Button表示未开启过的格子
					if (map[i, j].hasBoom) {
						// 如果踩到了雷，GameOver
						print ("GameOver!!");
					} else {
						// 否则展开（i,j）处格子
						Open (i, j);
					}
				}
			}
		}
	}

}
