import { useState, useEffect, useRef } from 'react';
import useSound from 'use-sound';
import { useAudioPlayer } from 'react-use-audio-player';
import IntroPopup from './IntroPopup';
import Voices from './Voice';
import './App.css';

//Importing background noises
import lvl1_back from './audio/BackingTracks/Level1.mp3';
import lvl2_back from './audio/BackingTracks/Level2.mp3';
import lvl3_back from './audio/BackingTracks/Level3.mp3';
import lvl4_back from './audio/BackingTracks/Level4.mp3';
import lvl5_back from './audio/BackingTracks/Level5.mp3';

//Clock ticking
import tickSFX from './audio/New/BackNoiseClips/464402__michael_grinnell__clock_tick_tock_loop.mp3';

//Background images
import StartImg from './images/StartScreen.png';
import BackgroundImg from './images/MainBackground.png';
import BackgroundImg1 from './images/Purple.png';
import BackgroundImg2 from './images/Blue.png';
import BackgroundImg3 from './images/Red.png';
import BackgroundImg4 from './images/Brown.png';
import Clock from './images/ClockFace.png';
import ClockHand from './images/ClockHand.png';

//Original Cusotmer
import NeutralImg from './images/MohawkNeutral.png';
import HappyImg from './images/MohawkHappy.png';
import AngryImg from './images/MohawkAngry.png';

//Muscle Customer
import NeutralMusImg from './images/MuscleNeutral.png';
import HappyMusImg from './images/MuscleHappy.png';
import AngryMusImg from './images/MuscleAngry.png';

//Sunglasses Customer
import NeutralSunImg from './images/SunGlassesNeutral.png';
import HappySunImg from './images/SunGlassesHappy.png';
import AngrySunImg from './images/SunGlassesAngry.png';

//Curly Customer
import NeutralCurlyImg from './images/CurlyNeutral.png';
import HappyCurlyImg from './images/CurlyHappy.png';
import AngryCurlyImg from './images/CurlyAngry.png';

//Fruit Icons (Importing of all fruit icons for player order)
import AppleIcon from './images/FruitIcons/Apple.png'
import BananaIcon from './images/FruitIcons/Banana.png'
import GrapeIcon from './images/FruitIcons/Grapes.png'
import MelonIcon from './images/FruitIcons/Melon.png'
import OrangeIcon from './images/FruitIcons/Orange.png'

//Consts for game settings
const GAME_COUNTDOWN = 60;
const LVL_REQ = 5;

const Game = () => {
  //Core Functionality Vars
  const [score, setScore] = useState(0);
  const [level, setLevel] = useState(0);
  const [failLevel, setFailLevel] = useState(false);
  const [orderSize, setOrderSize] = useState(3);
  const [lostCust, setLostCust] = useState(0);
  const [correctCust, setCorrectCust] = useState(0);
  const [totalCust, setTotalCust] = useState(0);
  const [order, setOrder] = useState([]);

  //  PlayerOrder (players order)
  const [playerOrder, setPlayerOrder] = useState([]);

  const [countdown, setCountdown] = useState(GAME_COUNTDOWN);

  //  Clock run with count down
  const [initialCountdown, setInitialCountdown] = useState(GAME_COUNTDOWN);
  const clockHandRef = useRef(null);

  const [orderInProgress, setOrderInProgress] = useState(false);
  const [gameStarted, setGameStarted] = useState(false);

  //  Popup
  const [showPopup, setShowPopup] = useState(false);
  const [introPopupState, setIntroPopupState] = useState(0);
  //  Pause
  const [gamePaused, setGamePaused] = useState(true);

  //Customer Image Arrays
  const normalCustomerImages = [
    NeutralImg,
    HappyImg,
    AngryImg
  ];

  const muscleCustomerImages = [
    NeutralMusImg,
    HappyMusImg,
    AngryMusImg
  ];

  const sungCustomerImages = [
    NeutralSunImg,
    HappySunImg,
    AngrySunImg
  ];

  const curlyCustomerImages = [
    NeutralCurlyImg,
    HappyCurlyImg,
    AngryCurlyImg
  ];

  const availableCustomer = [
    normalCustomerImages,
    muscleCustomerImages,
    sungCustomerImages,
    curlyCustomerImages,
  ];

  const [customerSpriteRef, setCustomerSpriteRef] = useState(muscleCustomerImages);
  const [customerState, setCustomerState] = useState(customerSpriteRef[0]);
  const [triggerNewCustomer, setTriggerNewCustomer] = useState(false);

  const [currentCustomerType, setCurrentCustomerType] = useState('Male2');
  const [playCorrect] = useSound(Voices[currentCustomerType].CorrectOrder);
  const [playWrong] = useSound(Voices[currentCustomerType].WrongOrder);
  const [playTick] = useSound(tickSFX, {volume: 0.5});

  const handlePlayCorrect = () => {
    playCorrect();
  };

  const handlePlayWrong = () => {
    playWrong();
  };

  //Order Voice Player
  const { load } = useAudioPlayer();
  const [orderSamples, setOrderSamples] = useState([]);
  const [orderIndex, setOrderIndex] = useState(-1);

  //Background Noises
  const [playLvl1] = useSound(lvl1_back);
  const [playLvl2] = useSound(lvl2_back);
  const [playLvl3] = useSound(lvl3_back);
  const [playLvl4] = useSound(lvl4_back);
  const [playLvl5] = useSound(lvl5_back);


  //Background Noises 
  const noiseList = [
    { sound: playLvl1 },
    { sound: playLvl2 },
    { sound: playLvl3 },
    { sound: playLvl4 },
    { sound: playLvl5 }
  ];

  //Background Images 
  const backList = [
    BackgroundImg1,
    BackgroundImg2,
    BackgroundImg3,
    BackgroundImg4
  ];

  const [backgroundState, setBackgroundState] = useState(StartImg);

  //Potential Order
  const orderList = [
    { name: "Apple", plural: "PluralApple" },
    { name: "Orange", plural: "PluralOrange" },
    { name: "Grape", plural: "PluralGrape" },
    { name: "Banana", plural: "PluralBanana" },
    { name: "Watermelon", plural: "PluralWatermelon" }
  ];

  //  Fruit icon mapping
  const fruitIcons = {
    Apple: AppleIcon,
    Banana: BananaIcon,
    Grape: GrapeIcon,
    Watermelon: MelonIcon,
    Orange: OrangeIcon,
  };

  //  OriginalWIdth and Height for the background
  const originalWidth = 3840;
  const originalHeight = 2160;
  //  Coords for the button area
  const [scaledCoords, setScaledCoords] = useState([]);
  const [highlightPoints, setHighlightPoints] = useState('');
  const imgRef = useRef(null);
  const svgRef = useRef(null);

  //  Coord when its original size
  const areas = [
    {
      name: 'Orange',
      coords: '639,1673,892,1673,916,1662,946,1658,974,1665,997,1675,1045,1678,1089,1678,1147,1675,997,1831,387,1834',
      onClick: () => handleFruitClick({ name: 'Orange', quantity: 1 })
    },
    {
      name: 'Apple',
      coords: '1218,1672,1262,1672,1272,1655,1299,1645,1303,1604,1343,1580,1384,1587,1418,1607,1445,1607,1465,1573,1499,1563,1533,1573,1560,1584,1601,1567,1632,1567,1659,1590,1676,1614,1676,1645,1683,1668,1710,1678,1808,1682,1778,1831,1225,1834,1197,1824,1164,1831,1140,1824,1099,1838,1086,1838',
      onClick: () => handleFruitClick({ name: 'Apple', quantity: 1 })
    },
    {
      name: 'Watermelon',
      coords: '2619,1672,2710,1668,2697,1621,2717,1563,2738,1516,2775,1475,2833,1441,2917,1428,2989,1431,3056,1468,3117,1539,3141,1584,3209,1526,3267,1526,3331,1546,3385,1570,3416,1621,3426,1658,3480,1692,3518,1733,3541,1750,3616,1743,3687,1767,3735,1814,3765,1841,3714,2113,3358,2119,3006,2126',
      onClick: () => handleFruitClick({ name: 'Watermelon', quantity: 1 })
    },
    {
      name: 'Grape',
      coords: '1024,1892,1072,1892,1072,1872,1089,1848,1113,1838,1136,1831,1160,1841,1184,1841,1201,1831,1221,1845,1235,1879,1228,1906,1479,1899,1523,1875,1547,1865,1584,1872,1611,1875,1642,1875,1676,1885,1700,1899,1767,1899,1737,2116,834,2123',
      onClick: () => handleFruitClick({ name: 'Grape', quantity: 1 })
    },
    {
      name: 'Banana',
      coords: '1822,1889,1937,1892,1957,1889,1984,1896,2056,1865,2076,1875,2117,1855,2151,1855,2215,1831,2181,1896,2208,1896,2232,1892,2280,1896,2293,1868,2317,1892,2473,1896,2459,1865,2473,1834,2497,1834,2517,1824,2554,1834,2592,1865,2636,1865,2660,1831,2687,1831,2700,1851,2704,1885,2894,2123,1791,2123',
      onClick: () => handleFruitClick({ name: 'Banana', quantity: 1 })
    }
  ];

  //  Fruit icon adjust
  const containerRef = useRef(null);
  const [maxFruitsPerRow, setMaxFruitsPerRow] = useState(4);

  //  Calculate the clock hand angle
  const updateClockHandRotation = () => {
    if (clockHandRef.current) {
      const rotationDegree = (360 / initialCountdown) * (initialCountdown - countdown);
      clockHandRef.current.style.transform = `rotate(${rotationDegree}deg)`;
    }
  };

  //  coords after scale
  const scaleCoords = (coords, widthRatio, heightRatio) => {
    return coords.split(',').map((coord, index) => {
      return index % 2 === 0
        ? Math.round(coord * widthRatio)
        : Math.round(coord * heightRatio);
    }).join(',');
  };

  //  Update coords after scale
  const updateScaledCoords = () => {
    if (imgRef.current) {
      const imgWidth = imgRef.current.getBoundingClientRect().width;
      const imgHeight = imgRef.current.getBoundingClientRect().height;

      const widthRatio = imgWidth / originalWidth;
      const heightRatio = imgHeight / originalHeight;

      const newScaledCoords = areas.map(area => ({
        ...area,
        coords: scaleCoords(area.coords, widthRatio, heightRatio)
      }));

      setScaledCoords(newScaledCoords);
    }
  };

  //  Hightlight
  const handleMouseEnter = (coords) => {
    setHighlightPoints(coords);
  };
  const handleMouseLeave = () => {
    setHighlightPoints('');
  };

  //  Update coords after run and start and change
  useEffect(() => {
    updateScaledCoords();
    window.addEventListener('resize', updateScaledCoords);

    return () => window.removeEventListener('resize', updateScaledCoords);
  }, []);

  //  Counts howmany fruit icon can be in one row
  useEffect(() => {
    const calculateFruitsPerRow = () => {
      if (containerRef.current) {
        const containerWidth = containerRef.current.offsetWidth;
        const fruitWidth = 25;
        const maxFruits = Math.floor((containerWidth - 50) / fruitWidth);
        setMaxFruitsPerRow(maxFruits);
      }
    };

    calculateFruitsPerRow();
    window.addEventListener('resize', calculateFruitsPerRow);

    return () => {
      window.removeEventListener('resize', calculateFruitsPerRow);
    };
  }, []);

  //Plays the background noise
  const playNoise = (level) => {
    console.log(level);
    noiseList[level].sound();
  };

  //Plays sounds of the current order
  const playOrderSoundsSequentially = () => {
    setOrderIndex(0);
  };

  useEffect(() => {
    if (orderIndex >= 0) {
      if (orderIndex === orderSamples.length) {
        setOrderIndex(-1);
        return;
      }
      load(orderSamples[orderIndex], {
        autoplay: true,
        onend: () => setOrderIndex(orderIndex + 1)
      });
    } else {
      return;
    }

  }, [orderIndex, load]);

  //Creates a new order for the user
  const orderCreate = () => {
    let newOrder = [];
    const availableFruits = [...orderList];
    let totalQuantity = 0;

    while (totalQuantity < orderSize && availableFruits.length > 1) {
      let randomIndex;
      let randomFruit;
      let quantity;
      if (availableFruits.length === 2 && (orderSize - totalQuantity) > 5) {
        randomIndex = Math.floor(Math.random() * availableFruits.length);
        randomFruit = availableFruits[randomIndex];

        quantity = 5;
      } else if (availableFruits.length === 1 && (orderSize - totalQuantity) > 5) {
        randomIndex = Math.floor(Math.random() * availableFruits.length);
        randomFruit = availableFruits[randomIndex];

        quantity = 5;
      } else {
        randomIndex = Math.floor(Math.random() * availableFruits.length);
        randomFruit = availableFruits[randomIndex];

        const maxPossibleQuantity = Math.min(5, orderSize - totalQuantity);
        quantity = Math.floor(Math.random() * maxPossibleQuantity) + 1;
      }

      newOrder.push({ name: randomFruit.name, quantity, plural: randomFruit.plural });

      availableFruits.splice(randomIndex, 1);
      totalQuantity += quantity;
    }

    setOrder(newOrder);

    // PlayerOrder (Reset the players order)
    setPlayerOrder([]);

    setOrderInProgress(true);

    setTotalCust(totalCust + 1);

    // Popup
    setShowPopup(false);
    // Pause
    setGamePaused(false);

    // Sets the order voices
    let soundOrder = [Voices[currentCustomerType].RequestOrder];

    for (let i = 0; i < newOrder.length; i++) {
      //adds and voceline
      if (newOrder.length > 1 && i === newOrder.length - 1) {
        soundOrder.push(Voices[currentCustomerType].And);
      }
      //adds number of fruit voice line
      if (newOrder[i].quantity === 1) {
        soundOrder.push(Voices[currentCustomerType].One);
      } else if (newOrder[i].quantity === 2) {
        soundOrder.push(Voices[currentCustomerType].Two);
      } else if (newOrder[i].quantity === 3) {
        soundOrder.push(Voices[currentCustomerType].Three);
      } else if (newOrder[i].quantity === 4) {
        soundOrder.push(Voices[currentCustomerType].Four);
      } else if (newOrder[i].quantity === 5) {
        soundOrder.push(Voices[currentCustomerType].Five);
      }

      //adds fruit name voice line
      if (newOrder[i].quantity === 1) {
        soundOrder.push(Voices[currentCustomerType][newOrder[i].name]);
      } else {
        soundOrder.push(Voices[currentCustomerType][newOrder[i].plural]);
      }

    }

    setOrderSamples(soundOrder);

    // Plays order sounds
    playOrderSoundsSequentially();
  };

  // Countdown function that function with the clock
  useEffect(() => {
    if (gameStarted && countdown > 0 && orderInProgress && !gamePaused) {
      const timer = setInterval(() => {
        setCountdown(prevCountdown => prevCountdown - 1);
      }, 1000);
      return () => clearInterval(timer);
    } else if (gameStarted && countdown === 0 && orderInProgress && !gamePaused) {
      setGamePaused(true);
      setOrderInProgress(false);
      setShowPopup(true);
      if (lostCust >= LVL_REQ) {
        setFailLevel(true);
      }
    }
  }, [countdown, orderInProgress, gameStarted, gamePaused]);

  // update the hand when countdown change
  useEffect(() => {
    updateClockHandRotation();
    playTick();
  }, [countdown]);


  // PlayerOrder (Add the fruit to the players order)
  const handleFruitClick = (fruit) => {
    setPlayerOrder((prevPlayerOrder) => {
      const existingFruit = prevPlayerOrder.find(item => item.name === fruit.name);

      if (existingFruit) {
        return prevPlayerOrder.map(item =>
          item.name === fruit.name ? { ...item, quantity: item.quantity + 1 } : item
        );
      } else {
        return [...prevPlayerOrder, { ...fruit, quantity: 1 }];
      }
    });
  };

  // Rendering player order
  {
    playerOrder.map((item, index) => (
      <div key={index} className="Fruit-Row">
        {[...Array(item.quantity)].map((_, i) => (
          <img
            key={i}
            src={fruitIcons[item.name]}
            alt={item.name}
            className="Fruit-Icon"
            style={{ left: `${i * 25}px` }}
          />
        ))}
      </div>
    ))
  };

  //  Clear the player order
  const handleResetOrder = () => {
    setPlayerOrder([]);
  };


  //  PlayerOrder (Handle the confirm of players order)
  const handleConfirmOrder = () => {
    let isCorrect = true;

    if (playerOrder.length !== order.length) {
      isCorrect = false;
    } else {
      playerOrder.forEach((fruit, index) => {
        if (!order[index] || fruit.name !== order[index].name || fruit.quantity !== order[index].quantity) {
          isCorrect = false;
        }
      });
    }

    if (isCorrect) {
      setScore(score + orderSize * 10);
      handlePlayCorrect();
      setCorrectCust(correctCust + 1);
      setCustomerState(customerSpriteRef[1]);
    } else {
      handlePlayWrong();
      setLostCust(lostCust + 1);
      setCustomerState(customerSpriteRef[2]);
    }
    setTriggerNewCustomer(true);
  };

  useEffect(() => {
    if (gameStarted && countdown > 0 && orderInProgress && !gamePaused && triggerNewCustomer) {
      let i = Math.floor(Math.random() * availableCustomer.length);
      setCustomerSpriteRef(availableCustomer[i]);
      if (i === 0) setCurrentCustomerType("Male1");
      if (i === 1) setCurrentCustomerType("Male2");
      if (i === 2) setCurrentCustomerType("Female1");
      if (i === 3) setCurrentCustomerType("Female2");

      setTriggerNewCustomer(false);
    }
  }, [triggerNewCustomer]);

  useEffect(() => {
    if (gameStarted && countdown > 0 && orderInProgress && !gamePaused) {
      setTimeout(() => {
        orderCreate();
        setCustomerState(customerSpriteRef[0]);
      }, 1000);
    }
  }, [customerSpriteRef]);

  //Randomly chooses background
  const randomBackground = () => {
    let randomIndex = Math.floor(Math.random() * backList.length);
    setBackgroundState(backList[randomIndex]);
  };

  //Starts the game
  const startGame = () => {
    setGameStarted(true);
    //  Popup
    setShowPopup(false);
    playNoise(level);
    randomBackground();
    orderCreate();
    setScore(0);
    updateScaledCoords();

    //  update the rotation of the clock
    updateClockHandRotation();
  };

  const startLevel = () => {
    setCountdown(GAME_COUNTDOWN);
    if (failLevel) {
      setLevel(0);
      setOrderSize(3);
      setFailLevel(false);
    } else if (level === noiseList.length - 1) {
      setLevel(0);
      setOrderSize(3);
    } else {
      setLevel(level => level + 1);
      setOrderSize(orderSize => orderSize + 1);
    }

    if (level !== null && orderSize >= 3 && gameStarted) {
      playNoise(level);
      randomBackground();
      orderCreate();
      setScore(0);
      updateScaledCoords();
      updateClockHandRotation();
    }
  };

  // Popup
  const handlePopupClose = () => {
    if (introPopupState === 0) {
      setIntroPopupState(1);
      return;
    } else if (introPopupState === 1) {
      setIntroPopupState(2);
      return;
    }

    setShowPopup(false);
    if (!gameStarted) {
      startGame();
    } else {
      startLevel();
    }
  };

  const handlePopupOpen = () => {
    // if (document.documentElement.requestFullscreen) {
    //   document.documentElement.requestFullscreen();
    // } else if (document.documentElement.mozRequestFullScreen) { // Firefox
    //   document.documentElement.mozRequestFullScreen();
    // } else if (document.documentElement.webkitRequestFullscreen) { // Chrome, Safari 和 Opera
    //   document.documentElement.webkitRequestFullscreen();
    // } else if (document.documentElement.msRequestFullscreen) { // IE/Edge
    //   document.documentElement.msRequestFullscreen();
    // }
    setShowPopup(true);
    setBackgroundState(BackgroundImg);
  };

  const reduceScore = () => {
    if (score <= 10) {
      setScore(0);
    }
    else {
      setScore(score - 10);
    }
  }

  return (
    <div className="App App-header">

      <div className='BackgroundHolder'>
        <img
          className="Background"
          srcSet={backgroundState}
          alt='The background of the game setting'
          useMap="#game-map"
          ref={imgRef}
        />

        {!gameStarted && !showPopup && (
          <>
            <button onClick={handlePopupOpen} className="Start-Button">Start</button>
            <a href='https://pastebin.com/2cNvVkhP' className='Start-Link Credit'>Audio Credits</a>
            <a href='https://yiliyapinya.org.au/' className='Start-Link Sponsor'>Main Page</a>
          </>
        )}

        {showPopup && (
          <div className="Popup">
            {countdown === 0 ? (
              <>
                {failLevel ? (
                  <>
                    <h2>Level {level + 1} Failed!</h2>
                    <p>Your current score is: {score}</p>
                    <p>The accuracy of your orders was: {correctCust - lostCust} / {totalCust}</p>
                    <p>You lost {lostCust} customers!</p>
                    <p>You can only lose less than {LVL_REQ} customers.</p>
                    <button onClick={handlePopupClose}>
                      Restart Level
                    </button>
                  </>
                ) : (
                  <>
                    <h2>Level {level + 1} Complete!</h2>
                    <p>Your current score is: {score}</p>
                    <p>The accuracy of your orders was: {correctCust - lostCust} / {totalCust}</p>
                    <p>You only lost {lostCust} customers!</p>
                    <p>You can only lose less than {LVL_REQ} customers.</p>
                    <button onClick={handlePopupClose}>
                      {level === noiseList.length - 1 ? 'Restart Game' : 'Next Level'}
                    </button>
                  </>
                )}
              </>
            ) : (
              <>
                <IntroPopup props={introPopupState} />
                <button onClick={handlePopupClose}>Next</button>
              </>
            )}
          </div>
        )}

        {gameStarted && orderInProgress && (
          <div className="Order-Container" ref={containerRef}>
            <div className='Order-List'>
              <p>Order</p>
              {playerOrder.map((item, index) => (
                <div key={index} className="Fruit-Row">
                  {[...Array(item.quantity)].map((_, i) => {
                    const isFirstInRow = i % maxFruitsPerRow === 0;
                    return (
                      <img
                        key={i}
                        src={fruitIcons[item.name]}
                        alt={item.name}
                        className="Fruit-Icon"
                        style={{ marginLeft: isFirstInRow ? '0' : '-25px' }}
                      />
                    );
                  })}
                </div>
              ))}
            </div>
            <button className="Confirm-Button" onClick={handleConfirmOrder}>Confirm</button>
            <button className="Confirm-Button" onClick={handleResetOrder}>Reset</button>
          </div>
        )}

        {gameStarted && (
          <div>
            <img className="Customer" srcSet={customerState} alt='The customer and their mood' />
            <button
              className="Produce-Buttons Repeat"
              onClick={() => {
                playOrderSoundsSequentially();
                reduceScore();
              }} disabled={showPopup}
            >Repeat</button>
          </div>
        )}

      </div>



      {/* {gameStarted && orderInProgress && (
        <>
          { <ul className="Debug">
            {order.map((item, index) => (
              <li key={index}>{item.quantity} x {item.name}</li>
            ))}
          </ul> }
        </>
      )} */}



      {gameStarted && (
        <div>
          <h2 className="Score">Score: {score}</h2>
          <div>
            <img className='Clock' srcSet={Clock} alt='The frame of the clock' />
            <img
              className='ClockHand'
              srcSet={ClockHand}
              alt='The hand of the clock'
              ref={clockHandRef}
            />
          </ div>
          {/* <h2 className="Countdown">Countdown: {countdown} secs</h2> */}

          <svg ref={svgRef} className="svg-overlay" width="100%" height="100%">
            {highlightPoints && (
              <polygon points={highlightPoints} fill="rgba(255, 255, 255, 0.4)" stroke="orange" strokeWidth="2" />
            )}
          </svg>
          <map name="game-map">
            {scaledCoords.map((area, index) => (
              <area
                key={index}
                shape="poly"
                coords={area.coords}
                alt={area.name}
                onClick={area.onClick}
                onMouseEnter={() => handleMouseEnter(area.coords)}
                onMouseLeave={handleMouseLeave}
              />
            ))}
          </map>

        </div>
      )}
    </div>
  );
};

export default Game;