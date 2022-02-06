function Auto::Netset::set( ) {

    $autonet::bottomprint = false;
    
    for (%i = 0; %i < 64; %i++) // how many item types are there anyway?
    {
        $autonet::terp[%i] = 64;
        $autonet::pft[%i] = 64;
        $autonet::pftMethod[%i] = 1;
    }

    // $autonet::terp[GetItemType("chaingun")] = 64;
    $autonet::pft[GetItemType("chaingun")] = 104;
    // $autonet::pftMethod[GetItemType("chaingun")] = 1;
    // $autonet::terp[GetItemType("chaingun")] = 64;
    $autonet::pft[GetItemType("disc launcher")] = 112;
    // $autonet::pftMethod[GetItemType("chaingun")] = 1;

    %weapon = GetMountedItem(0);
    $net::interpolatetime = $autonet::terp[%weapon];
    $net::predictforwardtimemethod = $autonet::pftMethod[%weapon];
    $net::predictforwardtime = $autonet::pft[%weapon];

    if ($autonet::bottomprint) {
    remoteBP(2048, "<JC>Terp: <F2>" ~ $net::interpolatetime ~ " <F1>PFT: <F2>" ~ $net::predictforwardtime, 1);}

    schedule::add("Auto::Netset::set();", 1);
}

Event::Attach(eventConnected, Auto::Netset::set);