
const IntroPopup = (props) => {
  if (props.props === 0) {
    return <>
      <h2>Purpose of the 'Say That Again' Game</h2>
      <p>This game improves your Brain Health by targeting:
        <br></br>
        <br></br>
        1) Auditory Perception - the ability to receive and interpret information through listening only
        <br></br>
        <br></br>
        2)  Phonological Short-term Memory - the ability to process sound stimuli we receive from the environment and retain this information for a short period of time.
        <br></br>
        <br></br>
        Your auditory perception and phonological short term memory is tested when you listen to customers orders and try to fulfill the requests, even with the background noises and competing sounds.
        <br></br>
        <br></br>
        Good luck!</p>
    </>;
  } else if (props.props === 1) {
    return <>
      <h2>Instructions</h2>
      <p>The aim of the game is to listen to the customer's order and accurately recreate it.</p>
      <p>Press the fruit X amount of times according to how many they want.</p>
      <p>Earn score based on order size!</p>
    </>;
  } else {
    return <>
      <h2>Be Careful!</h2>
      <p>
        Watch out for the clock!  You only have one minute to finish your orders!
        <br></br>
        <br></br>
        Take it as slow as you want!
        <br></br>
        Give 5 or more wrong orders and you must repeat the level!
        <br></br>
        <br></br>
        Try Your Best!
      </p>
    </>;
  }
};

export default IntroPopup;
