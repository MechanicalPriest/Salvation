import React, { useEffect } from 'react';
import useProfileDataManager from './Profile/useProfileDataManager';

export const GlobalContext = React.createContext();

// GlobalProvider acts as a global proxy between components and the data manager
export const GlobalProvider = ({ children }) => {

  // Grab the exports from the data mananger
  const {
    currentProfile,
    updateSpellValue,
  } = useProfileDataManager();

  // Add them to the output property for the context provider
  const provider = {
    currentProfile,
    updateSpellValue,
  };

  useEffect(() => {
    console.log('GlobalProvider currentProfile updated:', currentProfile);
  }, [currentProfile]);

  return (
    <GlobalContext.Provider value={provider}>{children}</GlobalContext.Provider>
  );
};
