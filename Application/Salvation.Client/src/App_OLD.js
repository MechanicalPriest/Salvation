import React, { useState, useEffect, useCallback } from "react";
import { makeStyles } from '@material-ui/core';
import { Button } from '@material-ui/core';
import { Grid } from '@material-ui/core';
import { Paper } from '@material-ui/core';
import { CircularProgress } from '@material-ui/core';
import TwoWayText from "./interface/TwoWayText.js";
import "./index.css";
import SpellComparison from "./Modules/SpellComparison";
import Highlight from 'react-highlight.js'
import Accordion from '@material-ui/core/Accordion';
import AccordionSummary from '@material-ui/core/AccordionSummary';
import AccordionDetails from '@material-ui/core/AccordionDetails';
import Typography from '@material-ui/core/Typography';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';

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
    //textAlign: 'center',
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
  const [modelResult, setModelResult] = useState({
    modelResults: {
      spec: 0,
      spellCastResults: [],
      rolledUpResultsSummary: [],
      profile: {}
    },
    statWeightsEffective: {},
    statWeightsRaw: {},
    journal: []
  });
  const [loading, setLoading] = useState(true);
  const [apiErrorMessage, setApiErrorMessage] = useState('');

  const profileDataUrl = process.env.REACT_APP_API_ROOT + '/api/DefaultProfile?specid=257';
  const modelResultUrl = process.env.REACT_APP_API_ROOT + '/api/ProcessModel';


  // make the API request to get default profile
  useEffect(() => {
    setLoading(true);
    console.log('Getting profile data from api:', profileDataUrl);
    fetch(profileDataUrl)
      .then((response) => response.json())
      .then(
        (data) => {
          console.log('Profile data received:', data);
          setProfileData(data);
          console.log('Set profile data:', profileData);
          setLoading(false);
        },
        (error) => {
          console.log(error);
          setLoading(false);
          setApiErrorMessage('Error connecting to API. See console for more details.');
        }
    );
    // TODO: Investigate actually using dependencies properly to remove these disables
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // Processing getting API results from submitted profile
  const handleUpdateClick = useCallback(() => {
    console.log('Sending profile data to modelling API:', JSON.stringify(profileData));

    fetch(modelResultUrl,
      {
        method: 'POST',
        body: JSON.stringify(profileData)})
      .then((response) => response.json())
      .then(
        (data) => {
          console.log('Modelling data received:', data);
          setModelResult(data);
          setLoading(false);
        },
        (error) => {
          console.log(error);
          setLoading(false);
          setApiErrorMessage('Error connecting to modelling API. See console for more details.');
        }
    );
    // TODO: Investigate actually using dependencies properly to remove these disables
    // eslint-disable-next-line react-hooks/exhaustive-deps
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
          <Grid item xs={12}>
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
              <Accordion>
                <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                  <Typography className={classes.heading}>Base Profile from API</Typography>
                </AccordionSummary>
                <AccordionDetails>
                  <Highlight language="json">
                    {JSON.stringify(profileData, null, 2)}
                  </Highlight>
                </AccordionDetails>
              </Accordion>              
            </Paper>
          </Grid>
          <Grid item xs={12}>
            <Paper className={classes.paper}>Results
              <SpellComparison data={modelResult} />
            </Paper>         
          </Grid>
        </Grid>
      </Grid> 
    </Grid>
  );
}

export default App;