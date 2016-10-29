# Launch

```
$ cd ~project_root/text/
$ lein run 3003
```

## Polarity Estimation


- for a word
  + go to `http://localhost:3003/polarity-estimation?word=いい&debug=true`
- for a text
  + got to `http://localhost:3003/polarity-estimation-from-text?text=隣の客はよく柿食う客だ&debug=true`
- Notion: this polarity should not be common word sense, but impression for a first time communication


## You can rebuild polarity dictionary

- check out `polarity.csv`
- then, restart your server ($ lein run 3003)

