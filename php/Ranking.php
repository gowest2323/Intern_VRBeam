<?php
//mysqli_report(MYSQLI_REPORT_ERROR | MYSQLI_REPORT_STRICT);

  $dbhost = 'localhost';
  $dbuser = 'ka';
  $dbpass = 'P9-INTERN_ka';
  $dbname = 'VRBeam';
  $mysqli = new mysqli($dbhost, $dbuser, $dbpass, $dbname);
  if ($mysqli->connect_errno)
  {
  echo "Failed to connect to MySQL: " . $mysqli->connect_error;
  }
  else{
    $mysqli->set_charset("utf8");
    //echo "good";
  }

  $ranking = $_REQUEST['ranking'];

  if($_REQUEST['action']=='show')
  {
    //表示//
    Show($mysqli, $ranking);
  }
  elseif($_REQUEST['action']=='checkIfRecordExist')
  {
    //記録があるかどうかを調べる//
    CheckIfRecordExist($mysqli, $ranking);
  }
  elseif($_REQUEST['action']=='checkSelfRank')
  {
    //自分が何位なのかを調べる//
    CheckSelfRank($mysqli, $ranking);
  }
  elseif($_REQUEST['action']=='submit')
  {
    //追加//
    Submit($mysqli, $ranking);
  }
  elseif($_REQUEST['action']=='update')
  {
    //更新//
    Update($mysqli, $ranking);
  }

  //接続を閉じる
  $mysqli->close();

/////////////////////////////////////////////////////////////

  //表示//
  function Show($mysqli, $ranking)
  {
    $limit = $_REQUEST['limit'];
    $query = "SELECT guid, score FROM $ranking ORDER BY score DESC LIMIT $limit";
    if ($result = $mysqli->query($query))
     {
       $num = $result->num_rows;
       if($num == 0)
       {
         printf("Don't have any record in %s\n", $ranking);
       }
       else
       {
         while ($row = $result->fetch_row())
         {
           printf("%s,", $row[0]);
           printf("%s,", $row[1]);
         }
      }
        $result->close();
    }
  }

  //記録があるかどうかを調べる//
  function CheckIfRecordExist($mysqli, $ranking)
  {
    $guid = $_REQUEST['guid'];
    $query = "SELECT guid, score FROM $ranking WHERE guid='$guid'";
    if ($result = $mysqli->query($query))
    {
      $num = $result->num_rows;
      if($num == 0)
      {
        printf("Don't have personal record in %s\n", $ranking);
      }
      else
      {
        while ($row = $result->fetch_row())
        {
          printf("%s,", $row[0]);
          printf("%s,", $row[1]);
        }
      }
      $result->close();
    }
  }

  //自分が何位なのかを調べる//
  function CheckSelfRank($mysqli, $ranking)
  {
    $guid = $_REQUEST['guid'];
    $query = "SELECT COUNT(id)+1 FROM $ranking WHERE score > (SELECT score FROM $ranking WHERE guid='$guid')";
    if ($result = $mysqli->query($query))
    {
      while ($row = $result->fetch_row())
      {
        printf("%s,", $row[0]);
      }
      /* free result set */
      $result->close();
    }

    $query = "SELECT guid, score FROM $ranking WHERE guid='$guid'";
    if ($result = $mysqli->query($query))
    {
      while ($row = $result->fetch_row())
      {
        printf("%s,", $row[0]);
        printf("%s,", $row[1]);
      }
      $result->close();
    }
  }

  //追加//
  function Submit($mysqli, $ranking)
  {
    $guid = $_REQUEST['guid'];
    $score = $_REQUEST['score'];
    $query = "INSERT INTO $ranking(id, guid, score) VALUES(null, '$guid', '$score')";
    if($mysqli->query($query)== TRUE)
    {
      printf("successfully inserted %s.%s to %s\n", $guid, $score, $ranking);
    }
    else
    {
      die( 'Error data set: ' . $mysqli->connect_error );
    }
  }

  //更新//
  function Update($mysqli, $ranking)
  {
    $guid = $_REQUEST['guid'];
    $score = $_REQUEST['score'];
    $query = "UPDATE $ranking SET score=$score WHERE guid='$guid'";
    if($mysqli->query($query)== TRUE)
    {
      printf("successfully updated %s.%s to %s\n", $guid, $score, $ranking);
    }
    else
    {
      die( 'Error data set: ' . $mysqli->connect_error );
    }
  }


  //倒した敵数ランキング////////////////////
  function KilledEnemyRankMode($mysqli, $action)
  {
    //表示////////////////////
    if($action=='show')
    {
      $limit = $_REQUEST['limit'];
      $query = "SELECT guid, score FROM KilledEnemyRanking ORDER BY score DESC LIMIT $limit";
      if ($result = $mysqli->query($query))
       {
          while ($row = $result->fetch_row())
          {
              printf("%s,", $row[0]);
              printf("%s,", $row[1]);
          }
          $result->close();
      }
    }

    //記録があるかどうかを調べる////////////////////
    if($action=='checkIfRecordExist')
    {
      $name = $_REQUEST['name'];
      $query = "SELECT guid, score FROM KilledEnemyRanking WHERE guid='$name'";
      if ($result = $mysqli->query($query))
      {
        $num = $result->num_rows;
        if($num == 0)
        {
          printf("Don't have this record.\n");
        }
        else
        {
          while ($row = $result->fetch_row())
          {
            printf("%s,", $row[0]);
            printf("%s,", $row[1]);
          }
        }
        $result->close();
      }
    }

    //自分が何位なのかを調べる////////////////////
    if($action=='checkSelfRank')
    {
      $name = $_REQUEST['name'];
      $query = "SELECT COUNT(id)+1 FROM KilledEnemyRanking WHERE killedEnemy > (SELECT killedEnemy FROM KilledEnemyRanking WHERE name='$name')";
      if ($result = $mysqli->query($query))
      {
        while ($row = $result->fetch_row())
        {
          printf("%s,", $row[0]);
        }
        /* free result set */
        $result->close();
      }

      $query = "SELECT name, killedEnemy FROM KilledEnemyRanking WHERE name='$name'";
      if ($result = $mysqli->query($query))
      {
        while ($row = $result->fetch_row())
        {
          printf("%s,", $row[0]);
          printf("%s,", $row[1]);
        }
        $result->close();
      }
    }


    //追加////////////////////
    if($action=='submit')
    {
      $name = $_REQUEST['name'];
      $killedEnemy = $_REQUEST['killedEnemy'];
      $query = "INSERT INTO KilledEnemyRanking(id, name, killedEnemy) VALUES(null, '$name', '$killedEnemy')";
      if($mysqli->query($query)== TRUE)
      {
        printf("successfully inserted %s.%s to KilledEnemyRanking\n", $name, $killedEnemy);
      }
      else
      {
        die( 'Error data set: ' . $mysqli->connect_error );
      }
    }

    //更新////////////////////
    if($action=='update')
    {
      $name = $_REQUEST['name'];
      $killedEnemy = $_REQUEST['killedEnemy'];
      $query = "UPDATE KilledEnemyRanking SET killedEnemy='$killedEnemy' WHERE name='$name'";
      if($mysqli->query($query)== TRUE)
      {
        printf("successfully updated %s.%s to KilledEnemyRanking\n", $name, $killedEnemy);
      }
      else
      {
        die( 'Error data set: ' . $mysqli->connect_error );
      }
    }
  }

  //生存WAVE数ランキング////////////////////
  function LivedWavesRankMode($mysqli, $action)
  {
    //表示////////////////////
    if($action=='show')
    {
      $limit = $_REQUEST['limit'];
      $query = "SELECT name, livedWaves FROM LivedWavesRanking ORDER BY livedWaves DESC LIMIT $limit";
      if ($result = $mysqli->query($query))
       {
          while ($row = $result->fetch_row())
          {
              printf("%s,", $row[0]);
              printf("%s,", $row[1]);
          }
          $result->close();
      }
    }

    //調べる////////////////////
    if($action=='checkIfRecordExist')
    {
      $name = $_REQUEST['name'];
      $query = "SELECT name, livedWaves FROM LivedWavesRanking WHERE name='$name'";
      if ($result = $mysqli->query($query))
      {
        $num = $result->num_rows;
        if($num == 0)
        {
          printf("Don't have this record.\n");
        }
        else
        {
          while ($row = $result->fetch_row())
          {
            printf("%s,", $row[0]);
            printf("%s,", $row[1]);
          }
        }
        $result->close();
      }
      //名前,スコア
    }

    //自分が何位なのかを調べる////////////////////
    if($action=='checkSelfRank')
    {
      $name = $_REQUEST['name'];
      $query = "SELECT COUNT(id)+1 FROM LivedWavesRanking WHERE livedWaves > (SELECT livedWaves FROM LivedWavesRanking WHERE name='$name')";
      if ($result = $mysqli->query($query))
      {
        while ($row = $result->fetch_row())
        {
          printf("%s,", $row[0]);
        }
        /* free result set */
        $result->close();
      }

      $query = "SELECT name, livedWaves FROM LivedWavesRanking WHERE name='$name'";
      if ($result = $mysqli->query($query))
      {
        while ($row = $result->fetch_row())
        {
          printf("%s,", $row[0]);
          printf("%s,", $row[1]);
        }
        $result->close();
      }
      //順位,名前,スコア
    }

    //追加////////////////////
    if($action=='submit')
    {
      $name = $_REQUEST['name'];
      $livedWaves = $_REQUEST['livedWaves'];
      $query = "INSERT INTO LivedWavesRanking(id, name, livedWaves) VALUES(null, '$name', '$livedWaves')";
      if($mysqli->query($query)== TRUE)
      {
        printf("successfully inserted %s.%s to LivedWavesRanking\n", $name, $livedWaves);
      }
      else
      {
        die( 'Error data set: ' . $mysqli->connect_error );
      }
    }

    //更新////////////////////
    if($action=='update')
    {
      $name = $_REQUEST['name'];
      $livedWaves = $_REQUEST['killedEnemy'];
      $query = "UPDATE LivedWavesRanking SET livedWaves='$livedWaves' WHERE name='$name'";
      if($mysqli->query($query)== TRUE)
      {
        printf("successfully updated %s.%s to LivedWavesRanking\n", $name, $livedWaves);
      }
      else
      {
        die( 'Error data set: ' . $mysqli->connect_error );
      }
    }
  }
?>
