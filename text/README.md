# Launch

```
$ cd project_root/text/
$ lein run 3003
```

## Polarity Estimation

- go to `localhost:3003/polarity-estimation?word=いい&debug=true`
- this polarity should not be common word sense, but impression for a first time communication

## You can rebuild polarity dictionary

- check out `polarity.csv`
- then, restart your server ($ lein run 3003)

