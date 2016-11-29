# Inteligentny algorytm do rozpoznawania i zliczania miejsc parkingowych na podstawie serii obrazów z kamery

Autor: Turski Sylwester

Promotor: Dr Hoser Paweł


## Streszczenie 



## Spis treści

  1. [Spis treści](#)
  2. [Wstęp](#)
  2. [Cel i zakres pracy](#)
  4. [Przegląd piśmiennictwa](#)
  5. [Materiały i metodykę pracy](#)
  6. [Omówienie i dyskusję wyników](#)
  7. [Podsumowanie i wnioski](#)
  8. [Spis piśmiennictwa](#)

## Wstęp
	
<!-- 
	nowoczesne społeczeństwo i inteligentne parkingi i nie tylko parkingi 
	problem z wolnymi miejscami parkingowymi w centrum miasta
	czujniki parkingowe kosztują, dając informację tylko o jednym miejscu
		kamera zbiera informację o kilku- kilkunastu miejscach 
	alternatywą jest uż
	
-->

## Cel i zakres pracy

<!--
	opracowanie algorytmu 
-->
## Przegląd piśmiennictwa
<!--
	https://www.youtube.com/watch?v=R9V1NCC6NPk&t=162s /* company project */
	https://www.youtube.com/watch?v=ypAg4PMEtso /* hakaton project *
	https://www.youtube.com/watch?v=KZIuMAhR4qU
	https://www.youtube.com/watch?v=iAy2ZWnnRAw
	https://www.youtube.com/watch?v=8A7vfMP0r7s
	https://www.youtube.com/watch?v=56AiTPYecTM
	https://www.youtube.com/watch?v=2RX7cjcFKLs&t=70s
	https://www.youtube.com/watch?v=G5dOHYmb8cc
	{
		https://www.youtube.com/watch?v=bPeGC8-PQJg
		https://www.youtube.com/watch?v=pEvd5FlELis
	}
-->
## Materiały i metodykę pracy
<!--
    metodyka agile
    mvp

    napisanie klasyfikatora hSv
        problem z szumem RGB
        binaryzacja 
            problem przy niskim V, 
                skalowanie s/v
            binaryzacja v>50% s>50%
        problem z samochodami o niskiej saturacji

    napisanie klasyfikatora liczebności krawędzi
        zliczanie pixeli z krawędzią
        problem z szumem RGB
        problem z samochodami zlewającymi się z tłem 
        problem z nielednolitym podłorzem (kostka brukowa)
        this - - > załorzenie o zdjęciach dobrej jakości 
    
    -------[Future]------------
    spreparowanie danych testowych do nauczenia klasyfikatora 
    sensitivity specificity roc curve
        dobranie odpowiednich wartości dla tresholdów 

    Nauczanie maszynowe 
        k-nn - metoda najbliższych sąsiadów 
        SVM - metoda vektorów nośnych
        
    wyliczanie dodatkowych cech 
        historgamy dla hSV
            wyliczanie wartości średniej i odchylenia standardowego

        badanie wykrytych krawędzi pod kątem ich długości i liczebności 
            histogram i badanie średniej i odchylenia standardowego
        badanie texture measurement haralick  
-->
## Omówienie i dyskusję wyników

<!--
    klasyfikator na podstawie hSv nie wykrywa samochodów o 
    kolorze białym szarym lub czarnym

    klasyfikator oparty o % krawędzi działa tak dobrze jak działa wykrywanie krawędzi
       klasyfikator przy pewnych warunkach oświetlenia i szumu RGB generowanego przez kamerę 
       nie jest w stanie wykryć czarnego samochodu
-->
## Podsumowanie i wnioski
<!--
	problem z samochodzmi o kolorze zbliżonym do koloru podłorza
	widok perspektywiczny 
-->
## Spis piśmiennictwa

#### Bibliografia:

- Smiatacz malina rozpoznawanie obrazów 
- Rozpoznawanie obrazów i mowy Kasprzak 
- Wstęp do sztucznej inteligencji flasinski 
- Digital Image processing handbook 
- Systemy uczące się krzyskio 
- Rozpoznawanie obrazów tadeusiewicz 