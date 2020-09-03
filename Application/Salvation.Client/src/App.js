import React, { useState, useEffect, useCallback } from "react";
import { makeStyles } from '@material-ui/core';
import { Button } from '@material-ui/core';
import { Grid } from '@material-ui/core';
import { Paper } from '@material-ui/core';
import { CircularProgress } from '@material-ui/core';
import TwoWayText from "./interface/TwoWayText.js";
import "./index.css";

const useStyles = makeStyles((theme) => ({
  root: {
    '& > *': {
      margin: theme.spacing(1),
      width: '25ch',
      display: 'flex',
      flexWrap: 'wrap',
      flexGrow: 1,
    }
  },
  textField: {
    marginLeft: theme.spacing(1),
    marginRight: theme.spacing(1),
    width: '25ch',
  },
  paper: {
    padding: theme.spacing(2),
    textAlign: 'center',
  },
}));

function App(props) {
  // Material styling
  const classes = useStyles();

  // Setup state variables
  const [profileData, setProfileData] = useState({
    intellect: 0,
    critRating: 0,
    hasteRating: 0,
    masteryRating: 0,
    versatilityRating: 0
  });
  const [loading, setLoading] = useState(true);
  const [apiErrorMessage, setApiErrorMessage] = useState('');

  const profileDataUrl = 'http://localhost:7071/api/DefaultProfile';

  // make the API request to get default profile
  useEffect(() => {
    setLoading(true);
    fetch(profileDataUrl)
      .then((response) => response.json())
      .then((data) => {
        console.log('Profile data received:', data);
        setProfileData(data);
        setLoading(false);
      })
      .catch(error => {
        console.log(error);
        setLoading(false);
        setApiErrorMessage('Error connecting to API. See console for more details.');
      });
  }, [profileDataUrl]);

  // Processing getting API results from submitted profile
  const handleUpdateClick = useCallback(() => {
    console.log('Sending profile data to API:', profileData);
  }, [profileData]);

  // Callback method to update a profile value when changed by a TextField
  const profileFieldChanged = (name, value) => {
    setProfileData(profileData => ({
      ...profileData,
      [name]: value
    }));
  };

  return (
    <Grid container className={classes.root} spacing={2}>
      <Grid item xs={12}>
        <Grid container justify="center" spacing={2}>
          <Grid item xs={12} >
            <img src="wings_banner_offset_small.png" alt="Mechanical Priest Banner"/>
          </Grid>
          <Grid item xs={6}>
            <Paper className={classes.paper}>
              <p>Enter stat ratings from character sheet</p>
              <TwoWayText label="Intellect" updateField={profileFieldChanged}
                value={profileData.intellect} name="intellect" className={classes.textField} />
              <TwoWayText label="Critical Strike" updateField={profileFieldChanged}
                value={profileData.critRating} name="critRating" className={classes.textField} />
              <TwoWayText label="Haste" updateField={profileFieldChanged}
                value={profileData.hasteRating} name="hasteRating" className={classes.textField} />
              <TwoWayText label="Mastery" updateField={profileFieldChanged}
                value={profileData.masteryRating} name="masteryRating" className={classes.textField} />
              <TwoWayText label="Versatility" updateField={profileFieldChanged}
                value={profileData.versatilityRating} name="versatilityRating" className={classes.textField} />
              <div>
                <Button onClick={handleUpdateClick} variant="contained" color="primary">Update</Button>
                <div>{loading === true && <CircularProgress />}</div>
                <div>{apiErrorMessage}</div>
              </div>
              <pre>{JSON.stringify(profileData)}</pre>
            </Paper>
          </Grid>
          <Grid item xs={6}>
            <Paper className={classes.paper}>Results</Paper>
          </Grid>
        </Grid>
      </Grid> 
    </Grid>
  );
}

export default App;