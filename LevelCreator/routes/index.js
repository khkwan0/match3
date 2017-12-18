var express = require('express');
var router = express.Router();

/* GET home page. */

router.get('/', function(req, res, next) {
  res.render('index', { title: 'Express' });
});

router.get('/levels', function(req, res, next) {
  let targetLevel = 0;
  if (req.query.level) {
    targetLevel = req.query.level - 1;
  }
  console.log(targetLevel);
  let Levels = req.db.collection('levels');
  Levels.findOne({level: targetLevel})
  .then((result) => {
    //console.log(result);
    res.status(200).send(JSON.stringify(result));
  })
  .catch((err) => {
    console.log(err.stack);
  })
});

module.exports = router;
