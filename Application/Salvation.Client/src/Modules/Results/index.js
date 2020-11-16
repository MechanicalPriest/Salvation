import { useSelector } from "react-redux";

const Results = () => {

  const baseResults = useSelector(state => state.resultsReducer.results);

  return (
    <div>
      <p>Results go here</p>
      {baseResults.modelResults !== undefined &&
        <p>HPS: {baseResults.modelResults.totalActualHPS}</p>
      }
    </div>
  );
};

export default Results;