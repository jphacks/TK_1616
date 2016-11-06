from linebot import LineBotApi
from pymongo import MongoClient
import settings
import feedback

channel_access_token = settings.ACCESS_TOKEN
line_bot_api = LineBotApi(channel_access_token)


def getreport(event):

    # get user information from user mid for fetching user name, which is displayed on              his/hers line account  
    # and from this information, fetch the final evaluation discription from mongoDB

    lineid = event.source.user_id
    profile = line_bot_api.get_profile(lineid)
    name = profile.display_name

    #connect to MongoDB and fetch evaluation data
    client = MongoClient(settings.DB_URI)
    db = client['comutrain']
    cols = db.feedback
    data = cols.find_one({'userLineID':name})

    if data is not None:
        # create message from template
        msg = feedback.makeReport(data)
    else:
        msg = "まだ会ったことないやん！"

    return msg
