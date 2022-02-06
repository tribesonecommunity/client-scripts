function GrabSpeed( %team, %cl ) {
	if ( getManagerId() == %cl ) {
		remoteBP( 2048, "<JC><F2>You <F1>grabbed going <F2>" ~ $Speed ~ " <F1>m/s!", 3 );
	}
}

Event::Attach(eventFlagGrab, GrabSpeed);