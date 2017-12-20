var express = require('express');
var router = express.Router();
var ObjectId = require('mongodb').ObjectID;
var fs = require('fs');

/* GET home page. */

router.get('/', function(req, res, next) {
  RenderIndex(1, req, res, next);
});

function RenderIndex(level, req, res, next) {
  let Levels = req.db.collection('levels');
  allLevels = [];
  Levels.find({}, {sort: {level: 1}}, {fields: {level: 1}})
  .then((result) => {
    for (var idx in result) {
      allLevels.push(result[idx].level);      
    }
    console.log(allLevels);
    res.render('index', { title: 'Match3 level editor', levels: allLevels, level: level });
  })
  .catch((err) => {
    console.log(err.stack);
  })
}

router.get('/level', (req, res, next)=> {
  if (req.query.level) {
    RenderIndex(req.query.level, req, res, next);
  } else {
    res.status(404).send();
  }
})

router.get('/levels', function(req, res, next) {
  let Levels = req.db.collection('levels');
  if (req.query.deploy) {
     Levels.find()
     .then((result) => {
       levelData = {
         levelData: result
       };
       fs.writeFile('../Assets/StreamingAssets/levels.json', JSON.stringify(levelData, null, 2), (err) => {
        if (err) {
          res.status(200).send(err);
        } else {
          res.status(200).send('ok');
        }
       });
     })
     .catch((err) => {
       console.log(err.stack);
       res.status(500).send(err.stack);
     })
  } else {
    let targetLevel = 0;
    if (req.query.level) {
      targetLevel = req.query.level - 1;
    }
    console.log(targetLevel);
    Levels.findOne({level: targetLevel})
    .then((result) => {
      //console.log(result);
      res.status(200).send(JSON.stringify(result));
    })
    .catch((err) => {
      console.log(err.stack);
    });
  }
});

router.post('/levels', (req, res, next) => {
  theBoard = JSON.parse(req.body.board);
  console.log(theBoard._id);
  let Levels = req.db.collection('levels')
  Levels.update(
    {_id: new ObjectId(theBoard._id)},
    theBoard,
    {
      upsert: true
    }
  )
  .then((result) => {
    res.status(200).send(JSON.stringify(result))
  })
  .catch((err) => {
    console.log(err.stack);
    res.status(200).send(JSON.stringify(err))
  })
});

router.get('/newlevel', (req, res, next) => {
  let Levels = req.db.collection('levels');
  Levels.find({}, {sort: {level: -1}, limit: 1}, {fields: {level: 1}})
  .then((result)=> {
    toInsert = {
      level: result[0].level + 1,
      rows: 8,
      cols: 8,
      numTileValues: 5,
      numMoves: 50,
      maxFillPoints: 4000,
      tier1Fill: 1000,
      tier2Fill: 2000,
      tier3Fill: 3000,
      mission: {
        type: 0,
        missionGoals: [{
          score: 2000
        }]
      },
      boardSpec: []
    }
    Levels.insert(toInsert)
    .then(() => {
      console.log('load level: ' + (result[0].level + 2));
      res.redirect('/level?level='+(result[0].level+2))
    })
    .catch((err) => {
      console.log(err.stack);
      res.status(500).send();
    })
  })
  .catch((err) => {
    console.log(err.stack);
    res.status(500).send();
  })
})

module.exports = router
