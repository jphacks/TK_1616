# -*- coding: utf-8 -*-

import json

sampleJsonStr = u'{"userPreference"  : {"keyword":"ポケモン", "point":10}, "disclosureReply" : {"action":"action1", "point":10},"userHobby"       : {"keyword":"釣り",    "point":0},"cousineAsk"      : {"action":"action3", "point":0},"askDinner"       : {"action":"action1", "point":10},"userLineID"      : "XXX","sum"             : 80}'

#print sampleJsonStr
sampleJson =  json.loads(sampleJsonStr)

def makeReport(feedbackJson):
    userPreference   = feedbackJson["userPreference"]
    disclosureReply  = feedbackJson["disclosureReply"]
    userHobby        = feedbackJson["userHobby"]
    cousineAsk       = feedbackJson["cousineAsk"]
    askDinner        = feedbackJson["askDinner"]
    kword1           = userPreference["keyword"]
    point1           = userPreference["point"]
    report1          = kword1+u"が好きなのはいいね。" if point1 >= 10 else kword1+u"が好きなのは微妙だったかな。"
    kword2           = userHobby["keyword"]
    point2           = userHobby["point"]
    report2          = kword2+u"が好きなのは魅力的だったよ。" if point2 >= 10 else kword2+u"が好きなのはあまり魅力的じゃなかったよ。"
    disclosureAction = disclosureReply["action"]
    disclosurePoint  = disclosureReply["point"]
    report3          = u"私の趣味について話してくれたのは嬉しかったよ。" if disclosurePoint >= 10 else kword2+u"私の趣味について詳しく聞いて欲しかったな。"
    cousineAction    = cousineAsk["action"]
    cousinePoint     = cousineAsk["point"]
    report4          = u"料理について話してくれたのはよかったかな。" if cousinePoint >= 0 else u"好きな料理について詳しく聞いて欲しかったな。"
    askDinnerAction  = askDinner["action"]
    askDinnerPoint   = askDinner["point"]
    report5          = u"料理に自然に誘えてたのは良かったね。" if askDinnerPoint > 0 else u"自然に料理に誘えなかったね。"
    report           = report1+u"それと"+report2+u"あと"+report3+u"他には"+report4+u"最後に"+report5
    return report

#print makeReport(sampleJson)

