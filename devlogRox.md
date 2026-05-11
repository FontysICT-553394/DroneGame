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

----------------------------------------------------------------------
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
- player movement gemaakt 
- Scherm die toont dat de drone uit het speelveld is. 
- Battery model die een soort currency is waarmee je punten verdient 
- score systeem 
- generation systeem aangepast op batteries die spawnen op lege nodes. 

## Waarom deze aanpak? 
- Ik heb ervoor gekozen om al snel score toe te voegen omdat je dan automatisch in de game gaat voor de batterijen pakken zo heb je een duidelijke doel, de game is nu klaar om bijvoorbeeld getest te worden. 
- Ik heb ervoor gekozen om toch geen death logic toe toe voegen omdat dit makkelijk is te implementeren en het nu toch zou worden gemaakt en meteen uitgezet. 

## Problemen
- Ik had vandaag redelijk wat problemen door mijn models, ik wou models met losse onderdelen zodat ze konden bewegen en dit had de pivot point van de model erg verschoven waardoor mijn model niet netjes op de node wou komen. 

## Oplossing
- Na wat te hebben gekeken naar mogelijkheden heb ik toch besloten om mijn models aan te passen en voor nu zonder animation te laten, ik ga hier later in development mee aan de slag aangezien het een "nice to have" onderdeel is. 

## Resultaten 
- Na werk van vandaag is de game helemaal speelbaar, het kan worden getoont aan de opdrachtgevers. 

## Reflectie 
- Ik heb vandaag veel kunnen oefenen met animeren en ik heb een aantal nieuwe inzichten gekregen in unity logica, zoals bijvoorbeeld gebruik van hashsets om alleen unieke node positions op te slaan. 

## Volgende stap
Als volgt ga ik werken aan het verfijnen van mijn obstakel logic die nu op elke node die leeg is een batterij zet, ik wil graag dast het ook een randomized aspect heeft. Ik wil ook graag nieuwe ground obstacles en air obstacles. Het zou ook leuk zijn als ik de finish kan maken. 

----------------------------------------------------------------------
# Devlog 3 - Roksana

## Datum 
- 10/05/2026

## Wat wa het doel? 
- Finish maken om de game "haalbaar" te maken. 
- nieuwe ground obstacle maken. 

## Wat heb ik gedaan? 
- Finish gemaakt + eindscherm 
- Nieuwe grond obstacle gemaakt: traffic cone 
- Nieuwe air obstacle gemaat: banner 
- Logica voor finish behalen en logica voor de nieuwe obstacles 

## Waarom deze aanpak? 
Ik heb ervoor gekozen om de finish te maken zodat de game haalbaar is, nu kunnen we instellen dat de finish na bv 2 minuten gameplay verschijnt. Ik heb ook extra models gemaakt om de game wat interessanter te maken. 

## Problemen
Ik heb dit keer geen problemen gehad bij het ontwikkelen van de nieuwe onderdelen. 

## Oplossing
-

## Resultaten 
Als resultaat hebben we een werkende finish logic en leuke nieuwe models 

## Reflectie 
Ik heb vandaag wat gespeeld met models maken en vooral nagedacht over welke models ik zou willen in de game 

## Volgende stap
Als volgt ga ik death logica ontwikkelen en wat extra models maken voor variatie 