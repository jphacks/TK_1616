# -*- coding: utf-8 -*-
import os
import sys

from argparse import ArgumentParser
from janome.tokenizer import Tokenizer

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
import messages

from doco.client import Client

from pymongo import MongoClient

#docomo conversation api
API_KEY = settings.API_KEY


app = Flask(__name__)

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
    user = {'nickname':'千歳', 'nickname_y':'チトセ', 'sex':'女', 'bloodtype':'B', 'birthdateY':'1996', 'birthdateM':12, 'birthdateD':15, 'age':20, 'constellations':'射手座', 'place':'東京'}
    c = Client(API_KEY, user=user)
    msg = event.message.text
    response = c.send(utt=msg, apiname="Dialogue")
    
    # information wihch is used for feching evaluation 
    lineid = event.source.user_id
    profile = line_bot_api.get_profile(lineid) 
    
    # if you sent a message saying "評価", you can get evaluation reply from line bot 
    # otherwise, you can just communicate with the bot
    if "評価" == event.message.text:
        line_bot_api.reply_message(
            event.reply_token,
            TextSendMessage(text=messages.evaluation)
        )
    else:
        line_bot_api.reply_message(
            event.reply_token,
            TextSendMessage(text=response['utt'])
        )


"""
    # get user information from user mid for fetching user name, which is displayed on his/hers line account
    # and from this information, fetch the final evaluation discription from mongoDB
    lineid = event.source.user_id
    profile = line_bot_api.get_profile(lineid)

    #connect to mongoDB
    client = MongoClient(settings.DB_URI)
    db = client['comutrain']
    cols = db.feedback

"""

@handler.add(FollowEvent)
def follow(event):

    # the message, witch is sent when user add this account.
    msg = messages.addfriend
    
    lineid = event.source.user_id
    profile = line_bot_api.get_profile(lineid)


    line_bot_api.reply_message(
        event.reply_token,
        TextSendMessage(text=msg)
    )

"""
    #connect to mongoDB
    client = MongoClient(settings.DB_URI)
    db = client['comutrain']
    cols = db.feedback
"""


    
if __name__ == "__main__":
    """
    arg_parser = ArgumentParser(
        usage='Usage: python ' + __file__ + ' [--port <port>] [--help]'
    )
    arg_parser.add_argument('-p', '--port', default=8000, help='port')
    arg_parser.add_argument('-d', '--debug', default=False, help='debug')
    options = arg_parser.parse_args()

    app.run(debug=options.debug, port=options.port)
    """
    app.run()
