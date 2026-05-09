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

## Extra werk in de avond 
In de avond heb ik nog thuis gewerkt aan de obstacles, ik heb nu een soort obstacle werkend, de logica zit er ook helemaal achter. Ik ben ook begonnen aan een asset die ervoor gaat zorgen dat de speler ook obstakels in de lucht heeft waardoor hij niet altijd over iets heen kan vliegen. De volgende stap gaat nu zijn om meer assets te maken die zullen dienen als obstacles en een dummy player om te kunnen beginnen aan movement logica en vanuit daar een death systeem bij het raken van een obstakel.

# Devlog 2 - Roksana

## Datum 
- 09/05/2026

## Wat wa het doel? 
- Model voor air obstacle 
- Air obstacle logic 
- Player movement logic 
- Death logic 

-- Bonus -- 
- 3rd ground obstacle model 
- 2nd air obstacle model 

## Wat heb ik gedaan? 
- Ik heb de lucht obstakel model afgemaakt en deze geimplementeerd zodat de drone niet over elke ground obstacle heen kan vliegen. 