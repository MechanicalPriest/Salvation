import React, { useState, useEffect } from "react";
import { TextField } from "@material-ui/core";

function TwoWayText(props) {
  // Bind props.value to value, and setValue to set the value.. redundant comments anyone?
  const [value, setValue] = useState(props.value);

  // When props.value changes, update the value variable
  useEffect(() => {
    setValue(props.value);
  }, [props.value]);

  // Handle the input being changed by calling the updateField method passed through as a prop
  const handleChange = (e) => {
    props.updateField(e.target.name, e.target.value);
  }

  return (
    <TextField name={props.name} onChange={handleChange} className={props.className} margin="normal" label={props.label} value={value} fullWidth />
  );
}

export default TwoWayText;