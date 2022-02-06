function EnemyHUD::GameBinds::Init()
	after GameBinds::Init
{
	$GameBinds::CurrentMapHandle = GameBinds::GetActionMap2( "actionMap.sae");
	$GameBinds::CurrentMap = "actionMap.sae";

	GameBinds::addBindCommand("EnemyHUD Toggle 1", "EnemyHUD::Toggle(1);");
	GameBinds::addBindCommand("EnemyHUD Toggle 2", "EnemyHUD::Toggle(2);");
	GameBinds::addBindCommand("EnemyHUD Toggle 3", "EnemyHUD::Toggle(3);");
	GameBinds::addBindCommand("EnemyHUD Toggle 4", "EnemyHUD::Toggle(4);");
	GameBinds::addBindCommand("EnemyHUD Toggle 5", "EnemyHUD::Toggle(5);");
}

function EnemyHUD::Init() {
	
	Hud::New::Shaded( "Enemy_Hud", 0, 200, 150, 84, EnemyHUD::Wake, EnemyHUD::Sleep );

	// row 1
	newObject("EnemyHUD::Slot1", FearGuiFormattedText, 5, 0, 140, 16);
	newObject("EnemyHUD::Slot2", FearGuiFormattedText, 5, 16, 140, 16);
	newObject("EnemyHUD::Slot3", FearGuiFormattedText, 5, 32, 140, 16);
	newObject("EnemyHUD::Slot4", FearGuiFormattedText, 5, 48, 140, 16);
	newObject("EnemyHUD::Slot5", FearGuiFormattedText, 5, 64, 140, 16);

	// row 2
	newObject("EnemyHUD::Slot6", FearGuiFormattedText, 125, 0, 140, 16);
	newObject("EnemyHUD::Slot7", FearGuiFormattedText, 125, 16, 140, 16);
	newObject("EnemyHUD::Slot8", FearGuiFormattedText, 125, 32, 140, 16);
	newObject("EnemyHUD::Slot9", FearGuiFormattedText, 125, 48, 140, 16);
	newObject("EnemyHUD::Slot10", FearGuiFormattedText, 125, 64, 140, 16);

	
	Hud::Add( "Enemy_Hud", "EnemyHUD::Slot1" );
	Hud::Add( "Enemy_Hud", "EnemyHUD::Slot2" );
	Hud::Add( "Enemy_Hud", "EnemyHUD::Slot3" );
	Hud::Add( "Enemy_Hud", "EnemyHUD::Slot4" );
	Hud::Add( "Enemy_Hud", "EnemyHUD::Slot5" );
	Hud::Add( "Enemy_Hud", "EnemyHUD::Slot6" );
	Hud::Add( "Enemy_Hud", "EnemyHUD::Slot7" );
	Hud::Add( "Enemy_Hud", "EnemyHUD::Slot8" );
	Hud::Add( "Enemy_Hud", "EnemyHUD::Slot9" );
	Hud::Add( "Enemy_Hud", "EnemyHUD::Slot10" );

}

function EnemyHUD::Wake() {
	EnemyHUD::Update();
}

function EnemyHUD::Sleep() {
	
}

function EnemyHUD::Toggle(%slot) {
	if(Control::GetVisible("EnemyHUD::Slot" @ %slot)) {
		Control::SetVisible("EnemyHUD::Slot" @ %slot, false);
		Control::SetVisible("EnemyHUD::Slot" @ %slot + 5, false);
		$EnemyTeamTemp::Name::Visible[%slot] = "False";
		$EnemyTeamTemp::Name[%slot] = $EnemyTeam::Name[%slot];
	}
	else {
		Control::SetVisible("EnemyHUD::Slot" @ %slot, true);
		Control::SetVisible("EnemyHUD::Slot" @ %slot + 5, true);
		$EnemyTeamTemp::Name::Visible[%slot] = "True";
		$EnemyTeamTemp::Name[%slot] = $EnemyTeam::Name[%slot];
	}
}

function EnemyHUD::UpdateTeams() {

	echo("Updating teams");
	$EnemyHUD::EnemyTeam = Team::Enemy();
	
	%myid = getManagerId();
	
	$EnemyHUD::Enemies = 0;

	$NewCount = 0;
	
	for( %l = 1; %l <= $Team::Client::Count; %l++ ) {
		
		%team = $Team::Client::Team[ $Team::Client::List[ %l ] ];
				
		if( %team == $EnemyHUD::EnemyTeam && $EnemyHUD::Enemies < 5) {
			$EnemyTeam::Name[ $EnemyHUD::Enemies++ ] = $Team::Client::Name[ $Team::Client::List[ %l ] ];
			$EnemyTeam::PlayerNumber[ $EnemyHUD::Enemies ] = %l;
		}
			
	}
	EnemyHUD::Update();
}

function EnemyHUD::Update() {

	for( %slot = 1; %slot <= $EnemyHUD::Enemies; %slot++ ) {
		if($EnemyHUD::Timer[ $EnemyTeam::PlayerNumber[ %slot] ] > 19)
			control::setValue( "EnemyHUD::Slot" @ %slot, "<f1>" ~ string::escapeFormatting($EnemyTeam::Name[ %slot ]) );
		else
			control::setValue( "EnemyHUD::Slot" @ %slot, string::escapeFormatting($EnemyTeam::Name[ %slot ]) );
	}

	for( %slot = 6; %slot <= $EnemyHUD::Enemies + 5; %slot++ ) {
		if($EnemyHUD::Timer[ $EnemyTeam::PlayerNumber[ %slot - 5 ] ] > 19)
			control::setValue( "EnemyHUD::Slot" @ %slot, "<f1>" ~ $EnemyHUD::Timer[ $EnemyTeam::PlayerNumber[ %slot - 5 ] ] );
		else
			control::setValue( "EnemyHUD::Slot" @ %slot, $EnemyHUD::Timer[ $EnemyTeam::PlayerNumber[ %slot - 5 ] ] );
	}

	for( %slot = $EnemyHUD::Enemies + 1; %slot <= 5; %slot++ )
		control::setValue( "EnemyHUD::Slot" @ %slot, "" );

	for( %slot = $EnemyHUD::Enemies + 6; %slot <= 10; %slot++ )
		control::setValue( "EnemyHUD::Slot" @ %slot, "" );

	Schedule::Add("EnemyHUD::UpdateTimer();",1);

}

function EnemyHUD::UpdateTimer() {

	for( %slot = 6; %slot <= $EnemyHUD::Enemies + 5; %slot++ ) {
		if($EnemyHUD::Timer[ $EnemyTeam::PlayerNumber[ %slot - 5 ] ] < 99)
			$EnemyHUD::Timer[ $EnemyTeam::PlayerNumber[ %slot - 5 ] ] = $EnemyHUD::Timer[ $EnemyTeam::PlayerNumber[ %slot - 5 ] ] + 1;

		if($EnemyHUD::Timer[ $EnemyTeam::PlayerNumber[ %slot - 5 ] ] > 19) {
			control::setValue( "EnemyHUD::Slot" @ %slot-5, "<f1>" ~ string::escapeFormatting($EnemyTeam::Name[ %slot - 5 ]) );
			control::setValue( "EnemyHUD::Slot" @ %slot, "<f1>" ~ $EnemyHUD::Timer[ $EnemyTeam::PlayerNumber[ %slot - 5 ] ] );
		}
		else {
			control::setValue( "EnemyHUD::Slot" @ %slot-5, string::escapeFormatting($EnemyTeam::Name[ %slot - 5]) );
			control::setValue( "EnemyHUD::Slot" @ %slot, $EnemyHUD::Timer[ $EnemyTeam::PlayerNumber[ %slot - 5 ] ] );
		}
	}
	
	Schedule::Add("EnemyHUD::UpdateTimer();",1);
	
}

function EnemyHUD::Reset() {
	
	for( %slot = 6; %slot <= $EnemyHUD::Enemies + 5; %slot++ ) {
		$EnemyHUD::Timer[ $EnemyTeam::PlayerNumber[ %slot - 5 ] ] = 0;
		control::setValue( "EnemyHUD::Slot" @ %slot, $EnemyHUD::Timer[ $EnemyTeam::PlayerNumber[ %slot - 5 ] ] );
	}

}

// suicides
function EnemyHUD::SetS_Status( %v, %w ) {
	if( Client::GetTeam( %v ) != $EnemyHUD::EnemyTeam )
		return;
	$EnemyHUD::Timer[ $Team::Client::Position[ %v ] ] = -2;
}

// deaths and team kills
function EnemyHUD::SetStatus( %k, %v, %w ) {
	if( Client::GetTeam( %v ) != $EnemyHUD::EnemyTeam )
		return;
	$EnemyHUD::Timer[ $Team::Client::Position[ %v ] ] = -2;
}

function EnemyHUD::TeamChange(%cl, %newteam) {
	
	$EnemyHUD::Timer[ $Team::Client::Position[ %cl ] ] = 0;

}

function EnemyHUD::SecondHalf( %cl, %msg, %type) after onClientMessage {
	if (((%msg == "Second half has started!.") || (%msg == "Second half has started!"))  && %type == 1) {
		EnemyHUD::Reset();
		for(%l=1; %l <= $EnemyHUD::Enemies; %l++) {
			%hidden = "False";
			for(%i = 1; %i <=5; %i++) {
				if($EnemyTeamTemp::Name::Visible[%i] == "False" && $EnemyTeamTemp::Name[%i] == $EnemyTeam::Name[%l]) {
					Control::SetVisible("EnemyHUD::Slot" @ %l, false);
					Control::SetVisible("EnemyHUD::Slot" @ %l + 5, false);
					%hidden = "True";
				}
				else if (%i == 5 && %hidden == "False")
				{
					Control::SetVisible("EnemyHUD::Slot" @ %l, true);
					Control::SetVisible("EnemyHUD::Slot" @ %l + 5, true);
				}
			}
			
		}
				

	}
}

EnemyHUD::Init();

Event::Attach( eventClientKilled, EnemyHUD::SetStatus );
Event::Attach( eventClientTeamKilled, EnemyHUD::SetStatus );
Event::Attach( eventClientSuicided, EnemyHUD::SetS_Status );
Event::Attach( eventClientsUpdated, EnemyHUD::UpdateTeams );
Event::Attach( eventClientChangeTeam, EnemyHud::TeamChange );
Event::Attach( eventConnected, EnemyHUD::Reset );
Event::Attach( eventMatchStarted, EnemyHUD::Reset );