import { useSelector } from "react-redux";

const Results = () => {

  const baseResults = useSelector(state => state.resultsReducer.results);

  return (
    <div>
      <p>Results go here</p>
      {baseResults.modelResults !== undefined &&
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
      }
    </div>
  );
};

export default Results;