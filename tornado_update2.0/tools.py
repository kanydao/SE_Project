# 检查所需的参数，避免传输过程中出现问题
account = 100000


def whether_miss(data, need):
    for it in need:
        if it not in data:
            return True
    return False


def new_account():
    global account
    account += 1
    acc = str(account)
    return acc


# 用于处理好友表，将好友表统一返回为一个list
# 返回字典方法，传入一个执行过查询sql语句的cursor对象

def sqlite_dict(obj_cursor):
    text = {}
    list = []
    # 获取obj_cursor的信息集
    data_set = obj_cursor.fetchall()
    # 将信息遍历
    for data in data_set:
        # 使用cursor游标的description方法，得到数据库的每一列的信息
        # 将data信息与其拼接为字典，并存放入列表中
        # 其实还有个数据库方法获取列名信息：PRAGMA TABLE INOF([表名])
        for s, x in enumerate(obj_cursor.description):
            text[x[0]] = data[s]
        # 若是多个引用值，list.append(str(text))
        list.append(text)
    # 返回列表
    return list


# 用来查询，好友列表中是否有发消息的用户
def check_friend(friend,f_list):
    flag = False
    for it in f_list:
        if friend == it['account']:
            flag = True
            break
    return flag