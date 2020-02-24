import React, { useContext, useState } from "react";
import { Tab, Header, Grid, Button } from "semantic-ui-react";
import { RootStoreContext } from "../../app/stores/rootStore";
import ProfileBioUpdateForm from "./ProfileBioUpdateForm";

const ProfileBio = () => {
  const rootStore = useContext(RootStoreContext);
  const { profile, isCurrentUser } = rootStore.profileStore;
  const [updateBioMode, setUpdateBioMode] = useState(false);

  return (
    <Tab.Pane>
      <Grid>
        <Grid.Column width={16}>
          <Header
            floated="left"
            icon="info"
            content={`About ${profile?.displayName}`}
          />
          {isCurrentUser && (
            <Button
              floated="right"
              basic
              content={updateBioMode ? "Cancel" : "Update bio"}
              onClick={() => setUpdateBioMode(!updateBioMode)}
            />
          )}
        </Grid.Column>
        <Grid.Column width={16}>
          {updateBioMode ? (
            <ProfileBioUpdateForm setUpdateBioMode={setUpdateBioMode} />
          ) : (
            <p>{profile!.bio}</p>
          )}
        </Grid.Column>
      </Grid>
    </Tab.Pane>
  );
};

export default ProfileBio;
