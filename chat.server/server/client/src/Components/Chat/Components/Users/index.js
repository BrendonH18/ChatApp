const Users = ({isConnectedUsersByChannelAndStatusTruthy, connectedUsersByChannelAndStatus}) => {
    return <>
    <div className="card mb-sm-3 mb-md-0 contacts_card">
        <div className="card-header msg_head">
            <div className="d-flex bd-highlight">
                <div className="img_cont">
                    <img src="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTC9pw-3QlI-doodCy0D-bsxEBZ9lFTcTFsZQ&usqp=CAU" className="rounded-circle user_img"/>
                    {isConnectedUsersByChannelAndStatusTruthy()
                        ? <span className="online_icon offline"></span> 
                        // ? <span className={connectedUsersByChannelAndStatus[channel.id]["Active"].length>0 ? "online_icon" : "online_icon offline"}></span>
                        : <></>}
                </div>
                <div className="user_info">
                    <span>Users</span>
                    {isConnectedUsersByChannelAndStatusTruthy()
                        ? <p> XX Connected</p>
                        // ? <p>{`${connectedUsersByChannelAndStatus[ActiveChannelID]["Active"].length} Connected`}</p>
                        : <></>}
                </div>
            </div>
        </div>
        <div className="card-body contacts_body">
            <ul className="contacts">
                {isConnectedUsersByChannelAndStatusTruthy()
                    ? [{user: {id: 1, username: "ONE"}}, {user: {id: 2, username: "TWO"}}].map(userConnection =>
                        <li id={`contact-${userConnection.user.id}`}>
                            <div className="d-flex bd-highlight" >
                                <div className="user_info" >
                                    <span >{userConnection.user.username}</span>
                                </div>
                            </div>
                        </li>
                    )
                // 	? connectedUsersByChannelAndStatus[ActiveChannelID]["Active"].map(userConnection =>
                // <li id={`contact-${userConnection.user.id}`}>
                // 	<div className="d-flex bd-highlight" >
                // 		<div className="user_info" >
                // 			<span >{userConnection.user.username}</span>
                // 		</div>
                // 	</div>
                // </li>
                // )
                    : <></>}
            </ul>
        </div>
        <div className="card-footer"></div>
    </div>
</>
}

export default Users