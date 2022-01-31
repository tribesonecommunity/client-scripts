// flag sounds for 1.40

$Flag::DropSound[friendlyflag]			="flag_dropF";
$Flag::DropSound[enemyflag]			="flag_dropE";

$Flag::CapSound[friendlyflag]			="flag_capE";
$Flag::CapSound[enemyflag]			="flag_capF";

$Flag::ReturnSound[friendlyflag]                ="flag_retF";
$Flag::ReturnSound[enemyflag]                   ="flag_retE";

function Flag::DropSounds( %team, %cl ) {

	return ( %team == Client::GetTeam( getManagerId() ) ) ? ( localSound( $Flag::DropSound[friendlyflag] ) ) : ( localSound( $Flag::DropSound[enemyflag] ) );
}

function Flag::ReturnSounds( %team, %cl ) {

	return ( %team == Client::GetTeam( getManagerId() ) ) ? ( localSound( $Flag::ReturnSound[friendlyflag] ) ) : ( localSound( $Flag::ReturnSound[enemyflag] ) );
}

function Flag::GrabSounds( %team, %cl ) {

	return ( %team == Client::GetTeam( getManagerId() ) ) ? ( localSound( $Flag::GrabSound[friendlyflag] ) ) : ( localSound( $Flag::GrabSound[enemyflag] ) );
}

function Flag::PickupSounds( %team, %cl ) {

	return ( %team == Client::GetTeam( getManagerId() ) ) ? ( localSound( $Flag::PickupSound[friendlyflag] ) ) : ( localSound( $Flag::PickupSound[enemyflag] ) );
}

function Flag::CapSounds( %team, %cl ) {

	return ( %team == Client::GetTeam( getManagerId() ) ) ? ( localSound( $Flag::CapSound[friendlyflag] ) ) : ( localSound( $Flag::CapSound[enemyflag] ) );
}

Event::Attach(eventFlagDrop, Flag::DropSounds);
Event::Attach(eventFlagReturn, Flag::ReturnSounds);
Event::Attach(eventFlagGrab, Flag::GrabSounds);
Event::Attach(eventFlagPickup, Flag::PickupSounds);
Event::Attach(eventFlagCap, Flag::CapSounds);