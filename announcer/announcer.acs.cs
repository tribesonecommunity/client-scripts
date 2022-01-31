Event::Attach( eventClientKilled, QuakeAnnouncer::onClientKilled );
Event::Attach( eventClientTeamKilled, QuakeAnnouncer::onClientTeamKilled );
Event::Attach( eventClientSuicided, QuakeAnnouncer::onClientSuicided );
Event::Attach( eventCountdownStarted, QuakeAnnouncer::onCountdownStarted );
Event::Attach( eventFlagInterception, QuakeAnnouncer::onFlagInt );
Event::Attach( eventFlagCatch, QuakeAnnouncer::onFlagCatch );
Event::Attach( eventFlagCap, QuakeAnnouncer::onFlagCap );
Event::Attach( eventFlagCarrierKill, QuakeAnnouncer::onFlagCarrierKill );
Event::Attach( eventFlagClutchReturn, QuakeAnnouncer::onFlagClutchReturn );
Event::Attach( eventFlagEGrab, QuakeAnnouncer::onFlagEGrab );
Event::Attach( eventFlagDrop, QuakeAnnouncer::onFlagDrop );
Event::Attach( eventFlagGrab, QuakeAnnouncer::onFlagGrab );
Event::Attach( eventFlagPickup, QuakeAnnouncer::onFlagPickup );
Event::Attach( eventFlagReturn, QuakeAnnouncer::onFlagReturn );
Event::Attach( eventMatchStarted, QuakeAnnouncer::onMatchStarted );
Event::Attach( eventMidAirCK, QuakeAnnouncer::onMidAirCK );
Event::Attach( eventMidAirDisc, QuakeAnnouncer::onMidAirDisc );
Event::Attach( eventMissionComplete, QuakeAnnouncer::onMissionComplete );
// Event::Attach( eventUpdateTime, QuakeAnnouncer::onUpdateTime);

$QuakeAnnouncer::DEBUG = false;

// Delay between sounds from the same event. When sounds are scheduled at the
// same time, they tend to clobber each other and nothing comes out. Increase if
// you want more space between messages
$QuakeAnnouncer::SOUND_BUFFER = 2;

// Time between kills to continue a kill streak
$QuakeAnnouncer::KILL_STREAK_TIME = 5;

// Time between consecutive team kills to continue a streak
$QuakeAnnouncer::TEAMKILL_TIME = 10;

// Time between when a flag was dropped and when it was caught
$QuakeAnnouncer::LONG_CATCH_TIME = 3;

// Time between when a flag was grabbed and when it was capped
// TODO: Test this
$QuakeAnnouncer::FAST_CAP_TIME = 15;

// Time between when a flag was picked up by a teammate and when it was capped
$QuakeAnnouncer::ASSIST_TIME = 5;

// Time between a flag return and a camp
$QuakeAnnouncer::CAMP_TIME = 4;

// Number of camps to be considered a camper
$QuakeAnnouncer::CAMP_THRESHOLD = 5;

// Number of caps to lead to consider a massacre
$QuakeAnnouncer::MASSACRE_THRESHOLD = 4;

// Number of consecutive air passes to be considered a streak
$QuakeAnnouncer::PASS_STREAK_THRESHOLD = 3;

// Track a team going on a streak
$QuakeAnnouncer::lastTeamToCap = -1;
$QuakeAnnouncer::capStreak[0] = 0;
$QuakeAnnouncer::capStreak[1] = 0;

// First blood!
$QuakeAnnouncer::firstCap = false;
$QuakeAnnouncer::firstKill = false;
$QuakeAnnouncer::countdownStarted = false;

function QuakeAnnouncer::debugEcho(%msg) {
   if ($QuakeAnnouncer::DEBUG) {
      echo(%msg);
   }
}

function QuakeAnnouncer::playSoundWithDelay(%sound, %delay) {
   QuakeAnnouncer::debugEcho("[QA::playSoundWithDelay:" @ %sound);
   if (%delay == 0) {
      localSound(%sound);
   } else {
      schedule("localSound(" @ %sound @ ");", %delay);
   }
}

function QuakeAnnouncer::playRandomSound2(%sound0, %sound1, %delay) {
   %i = floor(getRandom() * 2);
   if (%i == 0) {
      QuakeAnnouncer::playSoundWithDelay(%sound0, %delay);
   } else {
      QuakeAnnouncer::playSoundWithDelay(%sound1, %delay);
   }
}

function QuakeAnnouncer::playRandomSound3(%sound0, %sound1, %sound2, %delay) {
   %i = floor(getRandom() * 3);
   if (%i == 0) {
      QuakeAnnouncer::playSoundWithDelay(%sound0, %delay);
   } else if (%i == 1) {
      QuakeAnnouncer::playSoundWithDelay(%sound1, %delay);
   } else {
      QuakeAnnouncer::playSoundWithDelay(%sound2, %delay);
   }
}

function QuakeAnnouncer::playRandomSound4(%sound0, %sound1, %sound2, %sound3, %delay) {
   %i = floor(getRandom() * 4);
   if (%i == 0) {
      QuakeAnnouncer::playSoundWithDelay(%sound0, %delay);
   } else if (%i == 1) {
      QuakeAnnouncer::playSoundWithDelay(%sound1, %delay);
   } else if (%i == 2) {
      QuakeAnnouncer::playSoundWithDelay(%sound2, %delay);
   } else {
      QuakeAnnouncer::playSoundWithDelay(%sound3, %delay);
   }
}

function QuakeAnnouncer::playRandomSound5(%sound0, %sound1, %sound2, %sound3, %sound4, %delay) {
   %i = floor(getRandom() * 5);
   if (%i == 0) {
      QuakeAnnouncer::playSoundWithDelay(%sound0, %delay);
   } else if (%i == 1) {
      QuakeAnnouncer::playSoundWithDelay(%sound1, %delay);
   } else if (%i == 2) {
      QuakeAnnouncer::playSoundWithDelay(%sound2, %delay);
   } else if (%i == 3) {
      QuakeAnnouncer::playSoundWithDelay(%sound3, %delay);
   } else {
      QuakeAnnouncer::playSoundWithDelay(%sound4, %delay);
   }
}

function QuakeAnnouncer::onMatchStarted() {
   QuakeAnnouncer::debugEcho("[QA::onMatchStarted]");
   $QuakeAnnouncer::countdownStarted = false;
   $QuakeAnnouncer::capStreak[1] = 0;
   $QuakeAnnouncer::firstCap = false;
   $QuakeAnnouncer::firstKill = false;
   QuakeAnnouncer::playRandomSound5(
      "battle_begin_01",
      "battle_begin_02",
      "battle_begin_03",
      "battle_begin_04",
      "battle_begin_05"
   );
   schedule("QuakeAnnouncer::checkTime();", 30);
}

function QuakeAnnouncer::onCountdownStarted( %time ) {
   // Handle balanced mode switch
   %temp = $QuakeAnnouncer::capStreak[0];
   $QuakeAnnouncer::capStreak[0] = $QuakeAnnouncer::capStreak[1];
   $QuakeAnnouncer::capStreak[1] = %temp;

   QuakeAnnouncer::debugEcho("[QA::onCountdownStarted]: " @ %time);
   echo("[QA::onCountdownStarted]: " @ %time);
   $QuakeAnnouncer::countdownStarted = true;
   QuakeAnnouncer::playRandomSound4(
      "battle_prepare_01",
      "battle_prepare_02",
      "battle_prepare_03",
      "battle_prepare_04",
      1
   );
   if ( %time == 30 ) {
      QuakeAnnouncer::playSoundWithDelay("count_battle_10", 20);
   }
}

function QuakeAnnouncer::onClientKilled ( %killer, %victim, %damageType ) {
   QuakeAnnouncer::debugEcho("[QA::onClientKilled] killer:" @ %killer @ ", victim:" @ %victim  @ ", type:" @ %damageType);
   %count = 0;
   // if (!$QuakeAnnouncer::firstKill) {
   //    $QuakeAnnouncer::firstKill = true;
   //    QuakeAnnouncer::playRandomSound4(
   //       "firstblood_2",
   //       "firstblood_2",
   //       "firstblood_2",
   //       "firstblood_2",
   //       %count * $QuakeAnnouncer::SOUND_BUFFER
   //    );
   //    %count++;
   // }

   $QuakeAnnouncer::killSpree[ %victim ] = 0;

   %lastKillTime = $QuakeAnnouncer::lastKillTime[ %killer ];
   QuakeAnnouncer::debugEcho("[QA::onClientKilled] last kill time:", %lastKillTime);
   if (!%lastKillTime) {
      $QuakeAnnouncer::lastKillTime[ %killer ] = getSimTime();
      $QuakeAnnouncer::killStreak[ %killer ] = 1;
      $QuakeAnnouncer::killSpree[ %killer ] = 1;
   } else if (getSimTime() - %lastKillTime < $QuakeAnnouncer::KILL_STREAK_TIME) {
      $QuakeAnnouncer::lastKillTime[ %killer ] = getSimTime();
      $QuakeAnnouncer::killStreak[ %killer ] += 1;
      $QuakeAnnouncer::killSpree[ %killer ] += 1;
      if ($QuakeAnnouncer::killStreak[ %killer ] == 2) {
         QuakeAnnouncer::playSoundWithDelay("doublekill", %count * $QuakeAnnouncer::SOUND_BUFFER);
         %count++;
      } else if ($QuakeAnnouncer::killStreak[ %killer ] == 3) {
         QuakeAnnouncer::playSoundWithDelay("triplekill", %count * $QuakeAnnouncer::SOUND_BUFFER);
         %count++;
      } else if ($QuakeAnnouncer::killStreak[ %killer ] > 3) {
         QuakeAnnouncer::playSoundWithDelay("ultrakill", %count * $QuakeAnnouncer::SOUND_BUFFER);
         %count++;
      }
   } else {
      $QuakeAnnouncer::lastKillTime[ %killer ] = getSimTime();
      $QuakeAnnouncer::killStreak[ %killer ] = 1;
      $QuakeAnnouncer::killSpree[ %killer ] += 1;
      if ($QuakeAnnouncer::killSpree[ %killer ] > 2) {
         QuakeAnnouncer::playSoundWithDelay("killingspree", %count * $QuakeAnnouncer::SOUND_BUFFER);
         %count++;
      }
   }

   // if ($Collector::Kills[%killer] > 30) {
   //    localSound("wickedsick");
   // } else if ($Collector::Kills[%killer] > 35) {
   //    localSound("monsterkill");
   // } else if ($Collector::Kills[%killer] > 40) {
   //    localSound("holyshit");
   // }

   // if (%damageType == "Disc" && $Collector::Kills[Client::getName(%killer), %damageType] >= 25) {
   //    QuakeAnnouncer::playSoundWithDelay("diskjockey", %count * $QuakeAnnouncer::SOUND_BUFFER);
   //    %count++;
   // }
}

function QuakeAnnouncer::onClientTeamKilled ( %killer, %victim, %damageType ) {
   QuakeAnnouncer::debugEcho("[QA::onClientTeamKilled]");
   $QuakeAnnouncer::killSpree[ %victim ] = 0;

   if (!$QuakeAnnouncer::lastTeamKillTime [ %killer ]) {
      $QuakeAnnouncer::lastTeamKillTime[ %killer ] = getSimTime();
      $QuakeAnnouncer::teamKillStreak[ %killer ] = 1;
      return;
   }

   if (getSimTime() - $QuakeAnnouncer::lastTeamKillTime[ %killer ] < $QuakeAnnouncer::TEAMKILL_TIME) {
      $QuakeAnnouncer::lastTeamKillTime[ %killer ] = getSimTime();
      $QuakeAnnouncer::teamKillStreak[ %killer ] += 1;
      if ($QuakeAnnouncer::teamKillStreak[ %killer ] > 1) {
         localSound("teamkiller");
      }
   } else {
      $QuakeAnnouncer::lastTeamKillTime[ %killer ] = getSimTime();
      $QuakeAnnouncer::teamKillStreak[ %killer ] = 1;
   }
}

function QuakeAnnouncer::onClientSuicided ( %victim, %weapon ) { 
   QuakeAnnouncer::debugEcho("[QA::onClientSuicided]");
   $QuakeAnnouncer::killSpree[ %victim ] = 0;
}

function QuakeAnnouncer::onFlagCarrierKill ( %killer ) {
   QuakeAnnouncer::debugEcho("[QA::onFlagCarrierKill]");

   // There are a lot of kill events, so delay this one by default
   %count = 0;
   %carrierKills = $Collector::CarrierKills[Client::getName(%killer)];
   if (%carrierKills == 15) {
      QuakeAnnouncer::playSoundWithDelay(
         "dominating",
         %count * $QuakeAnnouncer::SOUND_BUFFER
      );
      %count++;
   } else if (%carrierKills == 20) {
      QuakeAnnouncer::playSoundWithDelay(
         "godlike",
         %count * $QuakeAnnouncer::SOUND_BUFFER
      );
      %count++;
   } else if (%carrierKills >= 25) {
      QuakeAnnouncer::playSoundWithDelay(
         "holyshit",
         %count * $QuakeAnnouncer::SOUND_BUFFER
      );
      %count++;
   }

   %killerTeam = Client::getTeam( %killer );
   $QuakeAnnouncer::lastCarrierKill[ %killerTeam ] = %killer;
}

function QuakeAnnouncer::onFlagClutchReturn ( %cl ) {
   QuakeAnnouncer::playSoundWithDelay("humiliation", $QuakeAnnouncer::SOUND_BUFFER);
}

function QuakeAnnouncer::onFlagEGrab ( %cl ) {
   QuakeAnnouncer::playSoundWithDelay("ninja", 0);
}

function QuakeAnnouncer::onFlagInt ( %team, %cl ) {
   QuakeAnnouncer::debugEcho("[QA::onFlagInt]");
   // If the client has a flag sound script, it will collide with these. If
   // you don't another flag sound script, you can set this delay to 0.
   // QuakeAnnouncer::playRandomSound2("impressive_1", "impressive_1", $QuakeAnnouncer::SOUND_BUFFER);
   if (%team == Team::Friendly()) {
      QuakeAnnouncer::playSoundWithDelay("denied", $QuakeAnnouncer::SOUND_BUFFER);
   } else {
      QuakeAnnouncer::playSoundWithDelay("denied", 0);
   }
   // if ($QuakeAnnouncer::lastCarrierKill[ %team ] == %cl) {
   //    QuakeAnnouncer::playRandomSound2("impressive_1", "impressive_1", $QuakeAnnouncer::SOUND_BUFFER);
   // } else {
   //    QuakeAnnouncer::playRandomSound2("intercepted", "interception", $QuakeAnnouncer::SOUND_BUFFER);
   // }
}

function QuakeAnnouncer::onFlagCap ( %team, %cl ) {
   QuakeAnnouncer::debugEcho("[QA::onFlagCap]", $QuakeAnnouncer::firstCap);
   %count = 0;

   if (!$QuakeAnnouncer::firstCap) {
      $QuakeAnnouncer::firstCap = true;
      QuakeAnnouncer::playSoundWithDelay(
         "firstblood_4",
         %count * $QuakeAnnouncer::SOUND_BUFFER
      );
      %count += 1;
   }

   // Individual cap performance
   %name = Client::getName(%cl);
   %caps = $Collector::Caps[%name];
   if (%caps == 3) {
      QuakeAnnouncer::playSoundWithDelay(
         "hattrick",
         %count * $QuakeAnnouncer::SOUND_BUFFER
      );
      %count += 1;
   } else if (%caps == 4) {
      QuakeAnnouncer::playSoundWithDelay(
         "unstoppable_1",
         %count * $QuakeAnnouncer::SOUND_BUFFER
      );
      %count += 1;
   } else if (%caps > 4) {
      QuakeAnnouncer::playSoundWithDelay(
         "rampage",
         %count * $QuakeAnnouncer::SOUND_BUFFER
      );
      %count += 1;
   }

   // Held flag without passing
   // if ($QuakeAnnouncer::lastFlagGrab[ %team ] && getSimTime() - $QuakeAnnouncer::lastFlagGrab[ %team ] < $QuakeAnnouncer::FAST_CAP_TIME) {
   //    QuakeAnnouncer::playSoundWithDelay(
   //       "speedfreak",
   //       %count * $QuakeAnnouncer::SOUND_BUFFER
   //    );
   //    %count += 1;
   // } else if ($QuakeAnnouncer::droppedFlag[ %team ] == false) {
   //    QuakeAnnouncer::playSoundWithDelay(
   //       "perfect",
   //       %count * $QuakeAnnouncer::SOUND_BUFFER
   //    );
   //    %count += 1;
   // }

   // Cap soon after pickup
   if ($QuakeAnnouncer::lastFlagPickup[ %team ] && $QuakeAnnouncer::lastFlagCarrier[ %team ]) {
      if (
         getSimTime() - $QuakeAnnouncer::lastFlagPickup[ %team ] < $QuakeAnnouncer::ASSIST_TIME &&
         $QuakeAnnouncer::lastFlagCarrier[ %team ] != %cl
      ) {
         QuakeAnnouncer::playSoundWithDelay(
            "assist",
            %count * $QuakeAnnouncer::SOUND_BUFFER
         );
         %count += 1;
      }
   }

   %flagTeam = %team;
   %cappingTeam = (%team + 1) % 2;
   %flagTeamCaps = Team::Score(%flagTeam);
   %cappingTeamCaps = Team::Score(%cappingTeam);

   // Lead changes
   if (%flagTeamCaps == %cappingTeamCaps) {
      QuakeAnnouncer::playSoundWithDelay(
         "teams_tied",
         %count * $QuakeAnnouncer::SOUND_BUFFER
      );
      %count += 1;
   } else if (%cappingTeam == %flagTeamCaps + 1) {
      if (%cappingTeam == Team::Friendly()) {
         QuakeAnnouncer::playSoundWithDelay(
            "taken_lead_1",
            %count * $QuakeAnnouncer::SOUND_BUFFER
         );
         %count += 1;
      } else {
         QuakeAnnouncer::playSoundWithDelay(
            "lost_lead_1",
            %count * $QuakeAnnouncer::SOUND_BUFFER
         );
         %count += 1;
      }
   }

   // Team cap streaks
   if ($QuakeAnnouncer::lastTeamToCap == -1) {
      $QuakeAnnouncer::lastTeamToCap = %team;
      $QuakeAnnouncer::capStreak[ %team ] = 1;
   } else if (%team == $QuakeAnnouncer::lastTeamToCap) {
      $QuakeAnnouncer::capStreak[ %team ] += 1;
      if ($QuakeAnnouncer::capStreak[ %team ] > 2) {
         QuakeAnnouncer::playSoundWithDelay(
            "ownage",
            %count * $QuakeAnnouncer::SOUND_BUFFER
         );
         %count += 1;
      }
   } else {
      $QuakeAnnouncer::capStreak[ %team ] = 1;
   }
   $QuakeAnnouncer::lastTeamToCap = %team;

   // Absolute lead
   if (
      %cappingTeamCaps >= %flagTeamCaps + $QuakeAnnouncer::MASSACRE_THRESHOLD
   ) {
      QuakeAnnouncer::playSoundWithDelay(
         "massacre",
         %count * $QuakeAnnouncer::SOUND_BUFFER
      );
      %count += 1;
   }

   // TODO: Make it so this isn't so hardcoded to work with balanced mode
   if (%flagTeam == Team::Enemy() && %cappingTeamCaps == 7 && %flagTeamCaps != 7) {
      QuakeAnnouncer::playRandomSound2(
         "finishit",
         "endit",
         %count * $QuakeAnnouncer::SOUND_BUFFER
      );
      %count += 1;
   }
}

function QuakeAnnouncer::onFlagDrop( %team, %cl ) {
   QuakeAnnouncer::debugEcho("[QA::onFlagDrop]");
   $QuakeAnnouncer::droppedFlag[ %team ] = true;
   $QuakeAnnouncer::lastFlagDrop[ %team ] = getSimTime();
}

function QuakeAnnouncer::onFlagGrab( %team, %cl ) {
   QuakeAnnouncer::debugEcho("[QA::onFlagGrab]");
   $QuakeAnnouncer::lastFlagGrab[ %team ] = getSimTime();
   $QuakeAnnouncer::lastCarrierKill[ %team ] = -1;

   $QuakeAnnouncer::droppedFlag[ %team ] = false;
   if (!$QuakeAnnouncer::lastFlagReturn[ %team ]) {
      return;
   }
   // if (getSimTime() - $QuakeAnnouncer::lastFlagReturn[ %team ] < $QuakeAnnouncer::CAMP_TIME) {
   //    $QuakeAnnouncer::camps[ %cl ] += 1;
   //    if ( $QuakeAnnouncer::camps[ %cl ] >= $QuakeAnnouncer::CAMP_THRESHOLD) {
   //       localSound("camper");
   //    }
   // }
}

function QuakeAnnouncer::onFlagPickup( %team, %cl ) {
   QuakeAnnouncer::debugEcho("[QA::onFlagPickup]");
   $QuakeAnnouncer::lastFlagPickup[ %team ] = getSimTime();
   $QuakeAnnouncer::lastFlagCarrier[ %team ] = %cl;
   if ($QuakeAnnouncer::wasCaught[ %team ] == false) {
      $QuakeAnnouncer::catchStreak[ %team ] = 0;
   }
   $QuakeAnnouncer::wasCaught[ %team ] = false;
   $QuakeAnnouncer::lastCarrierKill[ %team ] = -1;
}

function QuakeAnnouncer::onFlagReturn( %team, %cl ) {
   QuakeAnnouncer::debugEcho("[QA::onFlagReturn]");
   $QuakeAnnouncer::catchStreak[ %team ] = 0;
   $QuakeAnnouncer::lastFlagReturn[ %team ] = getSimTime();
}

function QuakeAnnouncer::onMissionComplete( %missionName ) {
   QuakeAnnouncer::debugEcho("[QA::onMissionComplete]");
   %firstTeamCaps = Team::Score(0);
   %secondTeamCaps = Team::Score(1);
   %count = 1;
   if (%firstTeamCaps == %secondTeamCaps) {
      QuakeAnnouncer::playRandomSound3(
         "humiliating",
         "laugh_1",
         "laugh_8",
         %count * $QuakeAnnouncer::SOUND_BUFFER
      );
      %count++;
   } else if (%firstTeamCaps == 8 || %secondTeamCaps == 8) {
      QuakeAnnouncer::playRandomSound3(
         "killingblow",
         "mercykill",
         "coupdegras",
         %count * $QuakeAnnouncer::SOUND_BUFFER
      );
      %count++;
   }

   if (%firstTeamCaps > %secondTeamCaps) {
      if (%firstTeamCaps >= %secondTeamCaps + $QuakeAnnouncer::MASSACRE_THRESHOLD) {
         QuakeAnnouncer::playSoundWithDelay(
            "smackdown",
            %count * $QuakeAnnouncer::SOUND_BUFFER
      );
         %count++;
      } else {
         if (Team::Friendly() == 0) {
            QuakeAnnouncer::playRandomSound2(
               "you_win",
               "congratulations",
               %count * $QuakeAnnouncer::SOUND_BUFFER
            );
            %count++;
         } else {
            QuakeAnnouncer::playRandomSound2(
               "you_lose",
               "loser",
               %count * $QuakeAnnouncer::SOUND_BUFFER
            );
            %count++;
         }
      }
   } else {
      if (%secondTeamCaps >= %firstTeamCaps + $QuakeAnnouncer::MASSACRE_THRESHOLD) {
         QuakeAnnouncer::playSoundWithDelay(
            "smackdown",
            %count * $QuakeAnnouncer::SOUND_BUFFER
         );
         %count++;
      } else {
         if (Team::Friendly() == 1) {
            QuakeAnnouncer::playRandomSound2(
               "you_win",
               "congratulations",
               %count * $QuakeAnnouncer::SOUND_BUFFER
            );
            %count++;
         } else {
            QuakeAnnouncer::playRandomSound2(
               "you_lose",
               "loser",
               %count * $QuakeAnnouncer::SOUND_BUFFER
            );
            %count++;
         }
      }
   }
}

function QuakeAnnouncer::onFlagCatch ( %team, %cl ) {
   QuakeAnnouncer::debugEcho("[QA::onFlagCatch]");
   %count = 0;

   $QuakeAnnouncer::wasCaught[ %team ] = true;

   if (getSimTime() - $QuakeAnnouncer::lastFlagDrop[ %team ] > $QuakeAnnouncer::LONG_CATCH_TIME) {
      QuakeAnnouncer::playSoundWithDelay(
         // "nicecatch", 
         "impressive_1", 
         %count * $QuakeAnnouncer::SOUND_BUFFER
      );
      %count++;
   }

   // Announce on multiple successive passes
   if (!$QuakeAnnouncer::catchStreak[ %team ]) {
      $QuakeAnnouncer::catchStreak[ %team ] = 1;
   } else {
      $QuakeAnnouncer::catchStreak[ %team ] += 1;
      if ($QuakeAnnouncer::catchStreak[ %team ] > $QuakeAnnouncer::PASS_STREAK_THRESHOLD) {
         QuakeAnnouncer::playSoundWithDelay(
            "excellent_1", 
            %count * $QuakeAnnouncer::SOUND_BUFFER
         );
         %count++;
      }
   }
}

function QuakeAnnouncer::onMidAirCK () {
   QuakeAnnouncer::debugEcho("[QA::onMidAirCK]");
   QuakeAnnouncer::playRandomSound2(
      "headshot_2",
      "headshot_2",
      0
   );
}

function QuakeAnnouncer::onMidAirDisc ( %shooter, %victim ) {
   QuakeAnnouncer::debugEcho("[QA::onMidAirDisc]");
   %maGiven = $Collector::MAGiven[ Client::getName( %shooter ) ];
   if (%maGiven == 15) {
      QuakeAnnouncer::playSoundWithDelay("headhunter", $QuakeAnnouncer::SOUND_BUFFER);
   } else if (%maGiven >= 20) {
      QuakeAnnouncer::playSoundWithDelay("nasty", $QuakeAnnouncer::SOUND_BUFFER);
   }
}

function QuakeAnnouncer::checkTime( %when ) {
   QuakeAnnouncer::debugEcho("[QA::checkTime] when:" @ %when @ ", secondsLeft:" @ $QuakeAnnouncer::secondsLeft);
   if ( %when != $QuakeAnnouncer::when ) {
      // The client has received a more recent time update
      return;
   }

   if (!$QuakeAnnouncer::countdownStarted) {
      if ( $QuakeAnnouncer::secondsLeft == 300 ) {
         localSound("cd5min");
      } else if ( $QuakeAnnouncer::secondsLeft == 180 ) {
         localSound("cd3min");
      } else if ( $QuakeAnnouncer::secondsLeft == 60) {
         localSound("cd1min");
      } else if ( $QuakeAnnouncer::secondsLeft == 30) {
         localSound("cd30sec");
      } else if ( $QuakeAnnouncer::secondsLeft == 10) {
         localSound("cd10");
      } else if ( $QuakeAnnouncer::secondsLeft == 9) {
         localSound("cd9");
      } else if ( $QuakeAnnouncer::secondsLeft == 8) {
         localSound("cd8");
      } else if ( $QuakeAnnouncer::secondsLeft == 7) {
         localSound("cd7");
      } else if ( $QuakeAnnouncer::secondsLeft == 6) {
         localSound("cd6");
      } else if ( $QuakeAnnouncer::secondsLeft == 5) {
         localSound("cd5");
      } else if ( $QuakeAnnouncer::secondsLeft == 4) {
         localSound("cd4");
      } else if ( $QuakeAnnouncer::secondsLeft == 3) {
         localSound("cd3");
      } else if ( $QuakeAnnouncer::secondsLeft == 2) {
         localSound("cd2");
      } else if ( $QuakeAnnouncer::secondsLeft == 1) {
         localSound("cd1");
      }
   }

   $QuakeAnnouncer::secondsLeft--;
   schedule("QuakeAnnouncer::checkTime(" @ %when @ ");", 1);
}

function QuakeAnnouncer::onUpdateTime( %min, %secs ) {
   QuakeAnnouncer::debugEcho("[QA::onUpdateTime] min:" @ %min @ ", sec:" @ %secs @ " , now:" @ getSimTime());
   $QuakeAnnouncer::secondsLeft = %min * 60 + %secs;
   $QuakeAnnouncer::when = getSimTime();
   // We seem to get whole numbers, but just in case
   $QuakeAnnouncer::secondsLeft = floor($QuakeAnnouncer::secondsLeft);
   schedule("QuakeAnnouncer::checkTime(" @ $QuakeAnnouncer::when @ ");", 1);
}