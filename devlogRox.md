# Devlog 1 - Roksana

## Datum
- 08/05/2026 

## Wat was de doel? 
- Ik wou vandaag graag de racing track klaar maken zodat ik altijd een bewegende track heb in beeld. 

## Wat heb ik gedaan? 
- Model gemaakt voor de racing tracks 
- Model als prefab gemaakt met benodigde componenten zoals: rigidbody & boxCollider 
- GameObject toegevoegd die dient als "finish" voor een racing track object
- 2 nieuwe scripts: 
        - DestroyRacingTrack: script voor het verwijderen van de track die de "finish" raakt
        - RacingTrackGenerator: script die ervoor zorgt dat er altijd 5 racing tracks in beeld zijn 

## Waarom deze aanpak? 
- Ik ben begonnen met de model omdat het simpel en low-poly is, dit heeft circa 30 minuten geduurd, zeer snel dus en zo kan ik de juiste afmetingen meteen gebruiken in mijn code voor het genereren van de tracks. 
- Daarna heb ik simpele scripts gemaakt die als basis zullen dienen voor de racing track logic. 

## Problemen 
- Ik had wat problemen met een for loop in de RacingTrackGenerator waardoor mijn game crashde. 

## Oplossing 
- Aangezien mijn hele unity crashde heb ik mijn code naar ChatGPT gestuurd, deze zag al meteen dat het aan een for loop ligt, ik heb dit aangepast en het werkte meteen. 

## Resultaten
Vandaag heb ik ervoor gezorgd dat je visueel kan zien dat de racing track beweegt, hij gaat oneindig door waarbij de niet meer zichtbare track delen worden verwijderd voor performance; 

## Reflectie
Ik heb vandaag geleerd hoe je een lopende track maakt waarbij je constant de eerste object verwijderd en de laatste erbij zet. 

## Volgende stap
De volgende stap is het neerzetten van obstakels op de track die worden random op de tracks gezet. 