<?php
$link = mysqli_connect("127.0.0.1", "admin_autobot","WjMKd1SmeCOsXkvZhd8VwyhKVrrq6H04rG4GclUgl7NR7T4eP7MCXluJ23S6AFsi");
$database = mysqli_select_db($link, "admin_autobot");

$currentDate = new DateTime();

$user = $_GET['username'];
$password_provided = $_GET['password'];
$tables = "licences";
$sql = "SELECT * FROM ". $tables ." WHERE username = '". mysqli_real_escape_string($link,$user) ."'" ;

$result = $link->query($sql);
if ($result->num_rows > 0) {
    // Outputting the rows
    while($row = $result->fetch_assoc())
    {
        function Redirect($url, $permanent = false)
        {
            if (headers_sent() === false)
            {
                header('Location: ' . $url, true, ($permanent === true) ? 301 : 302);
            }
        exit();
        }
		
		$endDate = new DateTime($row['license_expire']);            
		if ($currentDate > $endDate)
		{
			echo "489c7n495784c75n84"; //not expired
		}
        
        if ($password_provided == $row['password'])
        {
            if($row['banned_reason'] != "")
            {
                echo "x79347xm37489xm3"; // user banned
            }
            echo "x8457n84x5n784x5"; //password correct
        }
        else
        {
            echo "78934758973x97mx58"; //password incorrect
        }
        
        echo "g" . $row['license_type'];
    }
}  
?>