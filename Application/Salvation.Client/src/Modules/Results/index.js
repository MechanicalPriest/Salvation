import React, { useEffect } from 'react';
import { useSelector } from "react-redux";
import { makeStyles } from '@material-ui/core/styles';
import {
  TableContainer, Table, Paper, TableHead, TableRow, TableCell,
  TableBody, Link, Tooltip, Collapse, Box, Typography, IconButton
} from "@material-ui/core";
import KeyboardArrowDownIcon from '@material-ui/icons/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@material-ui/icons/KeyboardArrowUp';

const useStyles = makeStyles({
  table: {
    minWidth: 650,
  },
});

const Results = () => {

  const baseResults = useSelector(state => state.resultsReducer.results);
  const classes = useStyles();

  function tryRound(number) {
    var roundedNumber = parseFloat(parseFloat(number).toFixed(2));
    var formattedNumber = roundedNumber.toLocaleString(undefined, { maximumFractionDigits: 2 });
    return formattedNumber;
  }

  useEffect(() => {
    window.$WowheadPower.refreshLinks();
  }, [baseResults]);

  function Row(props) {
    const { cast } = props;
    const [open, setOpen] = React.useState(false);

    useEffect(() => {
      window.$WowheadPower.refreshLinks();
    }, [open]);

    return (
      <>
      <TableRow key={cast.spellId + cast.rawHealing}>
        <TableCell>
          {cast.additionalCasts.length > 0 &&
            <IconButton aria-label="expand row" size="small" onClick={() => setOpen(!open)}>
              {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
            </IconButton>
          }
        </TableCell>
        <TableCell component="th" scope="row">
          <Link target='_blank' rel='noreferrer' href={'//wowhead.com/spell=' + cast.spellId}>{cast.spellName}</Link>
        </TableCell>
        <TableCell align="right">{tryRound(cast.castsPerMinute)}</TableCell>
        <TableCell align="right">{tryRound(cast.rawHealing)}</TableCell>
        <TableCell align="right">{tryRound(cast.healing)}</TableCell>
        <TableCell align="right">{tryRound(cast.overhealing)}</TableCell>
        <TableCell align="right">{tryRound(cast.rawHPS)}</TableCell>
        <TableCell align="right">{tryRound(cast.hps)}</TableCell>
        <TableCell align="right">{tryRound(cast.rawHPM)}</TableCell>
        <TableCell align="right">{tryRound(cast.hpm)}</TableCell>
        <TableCell align="right">{tryRound(cast.rawHPCT)}</TableCell>
        <TableCell align="right">{tryRound(cast.hpct)}</TableCell>
        <TableCell align="right">{tryRound(cast.duration)}</TableCell>
        <TableCell align="right">{tryRound(cast.numberOfHealingTargets)}</TableCell>
        <TableCell align="right">{tryRound(cast.healing * (cast.castsPerMinute / 60) * baseResults.modelResults.timeToOom)}</TableCell>
        <TableCell align="right">{tryRound(cast.rawHealing * (cast.castsPerMinute / 60) * baseResults.modelResults.timeToOom)}</TableCell>
      </TableRow>
      {cast.additionalCasts.length > 0 &&
        <TableRow>
          <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={15}>
            <Collapse in={open} timeout="auto" unmountOnExit>
              <Typography variant="subtitle2" gutterBottom component="div">
                Additional effects for {cast.spellName}
          </Typography>
              <Box margin={1}>
                {renderTable(cast.additionalCasts)}
              </Box>
            </Collapse>
          </TableCell>
        </TableRow>
      }
      </>
    );
  }

  function renderTable(casts) {
    return (
      <Table className={classes.table} size="small" aria-label="Spell Casts">
        <TableHead>
          <TableRow>
            <TableCell></TableCell>
            <Tooltip title="Name of the spell" placement="bottom-start" arrow>
              <TableCell>Spell</TableCell>
            </Tooltip>
            <Tooltip title="Average casts per minute" placement="bottom-start" arrow>
              <TableCell>CPM</TableCell>
            </Tooltip>
            <Tooltip title="Raw healing done per cast" placement="bottom-start" arrow>
              <TableCell>Raw Heal</TableCell>
            </Tooltip>
            <Tooltip title="Actual healing done per cast (overheal)" placement="bottom-start" arrow>
              <TableCell>Heal</TableCell>
            </Tooltip>
            <Tooltip title="Overhealing done per cast" placement="bottom-start" arrow>
              <TableCell>Overhealing</TableCell>
            </Tooltip>
            <Tooltip title="Raw healing per second" placement="bottom-start" arrow>
              <TableCell>Raw HPS</TableCell>
            </Tooltip>
            <Tooltip title="Actual healing per second" placement="bottom-start" arrow>
              <TableCell>HPS</TableCell>
            </Tooltip>
            <Tooltip title="Raw healing per mana spent" placement="bottom-start" arrow>
              <TableCell>Raw HPM</TableCell>
            </Tooltip>
            <Tooltip title="Actual healing per mana spent" placement="bottom-start" arrow>
              <TableCell>HPM</TableCell>
            </Tooltip>
            <Tooltip title="Raw healing for time spent casting" placement="bottom-start" arrow>
              <TableCell>Raw HPCT</TableCell>
            </Tooltip>
            <Tooltip title="Actual healing for time spent casting" placement="bottom-start" arrow>
              <TableCell>HPCT</TableCell>
            </Tooltip>
            <Tooltip title="Duration of the proc/buff/spell" placement="bottom-start" arrow>
              <TableCell>Duration</TableCell>
            </Tooltip>
            <Tooltip title="Number of healing targets the spell hits" placement="bottom-start" arrow>
              <TableCell>Targets</TableCell>
            </Tooltip>
            <Tooltip title="Total healing done by the spell for the full fight length" placement="bottom-start" arrow>
              <TableCell>Total Healing</TableCell>
            </Tooltip>
            <Tooltip title="Total raw healing done by the spell for the full fight length" placement="bottom-start" arrow>
              <TableCell>Total Raw Healing</TableCell>
            </Tooltip>
          </TableRow>
        </TableHead>
        <TableBody>
          {casts.map((cast) => (
            <Row key={cast.spellId + cast.rawHealing} cast={cast} />
          ))}
        </TableBody>
      </Table>
    );
  }

  return (
    <div>
      <p>Results go here</p>
      {baseResults.modelResults !== undefined &&
        <div>
          <div>
            <p>HPS: {baseResults.modelResults.totalActualHPS}</p>
      
            <p>Stat Weights - {baseResults.statWeightsEffective.name}</p>
            {baseResults.statWeightsEffective.results.map((weight) => {
              return (
                <p>{weight.stat}: {parseFloat(weight.weight).toFixed(2)}</p>
              );
            })}
            <p></p>
          </div>
        <div>
          <p>Spell Results</p>
          <TableContainer component={Paper}>
            {renderTable(baseResults.modelResults.spellCastResults)}
          </TableContainer>
        </div>
      </div>
      }
    </div>
  );
};

export default Results;