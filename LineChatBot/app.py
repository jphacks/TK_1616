# -*- coding: utf-8 -*-
import os
import sys

from flask import Flask, request, abort
from linebot import (
    LineBotApi, WebhookHandler
)
from linebot.exceptions import (
    InvalidSignatureError
)
from linebot.models import (
    MessageEvent, FollowEvent, TextMessage, TextSendMessage,
)
import settings
import feedback
import get_feedback 
from doco.client import Client
from pymongo import MongoClient


app = Flask(__name__)

#docomo conversation api key
docomo_api_key = settings.DOCOMO_API_KEY
if docomo_api_key is None:
    print("DOCOMO_API_KEY not found")
    sys.exit(1)

# get channel_secret and channel_access_token from your environment variable
channel_secret = settings.CHANNEL_SECRET
channel_access_token = settings.ACCESS_TOKEN
if channel_secret is None:
    print('Specify LINE_CHANNEL_SECRET as environment variable.')
    sys.exit(1)
if channel_access_token is None:
    print('Specify LINE_CHANNEL_ACCESS_TOKEN as environment variable.')
    sys.exit(1)


#process line api keys
line_bot_api = LineBotApi(channel_access_token)
handler = WebhookHandler(channel_secret)

#process docomo api key
user = settings.user_config
doco = Client(docomo_api_key, user=user)


@app.route("/callback", methods=['POST'])
def callback():
    # get X-Line-Signature header value
    signature = request.headers['X-Line-Signature']

    # get request body as text
    body = request.get_data(as_text=True)
    app.logger.info("Request body: " + body)

    # handle webhook body
    try:
        handler.handle(body, signature)
    except InvalidSignatureError:
        abort(400)

    return 'OK'




@handler.add(MessageEvent, message=TextMessage)
def message_text(event):
    
    #docomo dialogue api
    msg = event.message.text
    response = doco.send(utt=msg, apiname="Dialogue")


    # when you sent a specific message, you can get evaluation reply from line bot 
    # otherwise, you can just communicate with the bot
    if "評価" == event.message.text:
        
        # get message from MongoDb
        msg = get_feedback.getreport(event)

        # send an evaluation message
        line_bot_api.reply_message(
            event.reply_token,
            TextSendMessage(text=msg)
        )
    else:
        line_bot_api.reply_message(
            event.reply_token,
            TextSendMessage(text=response['utt'])
        )


# it starts when user follow this bot
@handler.add(FollowEvent)
def follow(event):


    #get message from MongoDb
    msg = get_feedback.getereport(event)
    
    # create message, which is sent when user add this bot as a friend
    msg = "LINE追加ありがとう(happy)\n" + msg

    # send an evaluation message
    line_bot_api.reply_message(
        event.reply_token,
        TextSendMessage(text=msg)
    )

    
if __name__ == "__main__":
    app.run()
