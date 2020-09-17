import React from "react";

// Stuff from Material-UI
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableContainer from '@material-ui/core/TableContainer';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import Highlight from 'react-highlight.js'
import Accordion from '@material-ui/core/Accordion'
import AccordionSummary from '@material-ui/core/AccordionSummary'
import AccordionDetails from '@material-ui/core/AccordionDetails'
import ExpandMoreIcon from '@material-ui/icons/ExpandMore'

function round(value, precision = 2) {
  return value.toFixed(precision);
}

function SubRow(props) {
  const { row } = props;
  const { depth } = props;

  const dashes = String.fromCharCode(0x2014).repeat(depth)

  return (
    <React.Fragment>
      <TableRow>
        <TableCell>{dashes}&gt;</TableCell>
        <TableCell>
          {row.spellName}
        </TableCell>
        <TableCell align="right">{round(row.rawHealing, 0)}</TableCell>
        <TableCell align="right">{round(row.castsPerMinute, 1)}</TableCell>
        <TableCell align="right">{round(row.maximumCastsPerMinute, 1)}</TableCell>
        <TableCell align="right">{round(row.rawHPS, 2)}</TableCell>
      </TableRow>
      {row.additionalCasts.map((subRow) => (
        <SubRow key={row.spellId + "." + subRow.spellId} row={subRow} depth={depth + 1} />
      ))}
    </React.Fragment>
  )
}

function Row(props) {
  const { row } = props;

  return (
    <React.Fragment>
      <TableRow key={row.spellId}>
        <TableCell>&mdash;</TableCell>
        <TableCell component="th" scope="row">
          {row.spellName}
        </TableCell>
        <TableCell align="right">{round(row.rawHealing, 0)}</TableCell>
        <TableCell align="right">{round(row.castsPerMinute, 1)}</TableCell>
        <TableCell align="right">{round(row.maximumCastsPerMinute, 1)}</TableCell>
        <TableCell align="right">{round(row.rawHPS, 2)}</TableCell>
      </TableRow>
      {row.additionalCasts.map((subRow) => (
        <SubRow key={row.spellId + "." + subRow.spellId} row={subRow} depth={1} />
      ))}
    </React.Fragment>
  );
}

function SpellComparison(props) {
  const results = props.data.modelResults.spellCastResults

  return (
    <div>
      <TableContainer component={Paper}>
        <Table aria-label="collapsible table" size="small">
          <TableHead>
            <TableRow>
              <TableCell />
              <TableCell>Spell</TableCell>
              <TableCell align="right">Raw Healing</TableCell>
              <TableCell align="right">CPM</TableCell>
              <TableCell align="right">Max CPM</TableCell>
              <TableCell align="right">Raw HPS</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {results.map((row) => (
              <Row key={row.spellName} row={row} />
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <Accordion>
        <AccordionSummary expandIcon={<ExpandMoreIcon />}>
          <Typography>Modelling response from API</Typography>
        </AccordionSummary>
        <AccordionDetails>
          <Highlight language="json">
            {JSON.stringify(props.data, null, 2)}
          </Highlight>
        </AccordionDetails>
      </Accordion> 
    </div>
  );
}

export default SpellComparison;