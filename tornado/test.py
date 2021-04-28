# # 测试程序，测试向后端发送json数据，同时收到后端的json数据，将其解析显示以来验证后端代码
# import websocket
# import json
#
# # 建立一个websocket连接
# ws = websocket.create_connection("ws://8.140.2.201:8000/main")
# # 对websocket客户端发送一个请求
# a = {
#     "function":"login",
#     "data":
#     {
#         "account":"10001",
#         "password":"0b913be07bfabaee6c9b97cfc3bd101a",
#     }
# }
# a = json.dumps((a))
# ws.send(a)
# # 接返回的数据
# result =ws.recv()
# print(result)
# 开始测试注册时返回好友信息模块
import sqlite3
conn = sqlite3.connect("./db/NEUIM.db")  # 不存在的话，默认创建一个数据库
print("open successfully!")
cur = conn.cursor()
# sql ='''INSERT INTO Friend (account_1,account_2) VALUES('100001','100002')'''
# sql2 = '''INSERT INTO Friend (account_1,account_2) VALUES('100001','100003')'''
#sql2 = '''INSERT INTO Friend (account_1,account_2) VALUES('100004','100001')'''
#cur.execute(sql)
sql2 = '''SELECT * From Friend'''
sql3 = '''SELECT username,account,icon,state FROM User WHERE account in
        (SELECT account_1 FROM Friend WHERE account_2 = {}  )'''.format(100001)
cur.execute(sql3)
res2 = cur.fetchall()
print(res2)
cur.close()
# data={
#     "account": "100002",
#     "password": "0b913be07bfabaee6c9b97cfc3bd101a"
# }
# sql = '''SELECT username ,icon FROM User WHERE account='{}' LIMIT 1'''.format(data['account'])
# cur = conn.cursor()
# cur.execute(sql)
# res = cur.fetchone()  # 去数据库中查询相应的数据
# print(res)
