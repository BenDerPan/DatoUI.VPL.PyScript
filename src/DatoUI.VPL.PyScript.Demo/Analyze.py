import os
from urllib.parse import urlparse
import requests
import numpy as np
import matplotlib.pyplot as plt
import clr
clr.AddReference("System.Windows.Forms")
from System.Windows.Forms import MessageBox

class Analyze:
    def __init__(self):
        pass

    def run(self):
        url=urlparse("http://baidu.com")
        print("Analyze:{0}".format(url.hostname))
        r=requests.get("https://baidu.com")
        MessageBox.Show("这是弹窗\r\b"+r.text)

    def show_graph(self):
        
        x = np.array([1,2,3,4,5,6,7,8])
        y = np.array([3,5,7,6,2,6,10,15])
        plt.plot(x,y,'r')# 折线 1 x 2 y 3 color
        plt.plot(x,y,'g',lw=10)# 4 line w
        # 折线 饼状 柱状
        x = np.array([1,2,3,4,5,6,7,8])
        y = np.array([13,25,17,36,21,16,10,15])
        plt.bar(x,y,0.2,alpha=1,color='b')# 5 color 4 透明度 3 0.9
        plt.show()
