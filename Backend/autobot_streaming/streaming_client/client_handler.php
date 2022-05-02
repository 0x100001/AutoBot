<?php
$link = mysqli_connect("localhost", "owtnkjqx_6643fhju","btimTRT9TRxhl0vjrmd0");
$database = mysqli_select_db($link, "owtnkjqx_zhegz");

$user = $_GET['username'];
$token_provided = $_GET['token'];

//Spotify Requests
$spotify_login_username_pattern_request = $_GET['8937m83x74893']; //spotify login username pattern
$spotify_login_password_pattern_request = $_GET['756nx46453646']; //spotify login password pattern
$spotify_login_button_pattern_request = $_GET['783x7834x3x43']; //spotify login button pattern
$spotify_login_error_pattern_request = $_GET['x38x482934833']; //spotify login button pattern
$spotify_login_verify_pattern_request = $_GET['4834783748344']; //spotify login button pattern
$spotify_play_button_pattern_request = $_GET['n753c6n7c5c536']; //spotify login button pattern
$spotify_skip_button_pattern_request = $_GET['x473x9783x783']; //spotify login button pattern

//Amazon Requests
$amazon_signup_button_pattern_request = $_GET['7894n63576n756n7']; //spotify login username pattern
$amazon_login_username_pattern_request = $_GET['847cm4758948c']; //spotify login username pattern
$amazon_login_password_pattern_request = $_GET['cm8475m8947c584c']; //spotify login password pattern
$amazon_login_button_pattern_request = $_GET['78c47m854c7m5']; //spotify login button pattern
$amazon_play_button_pattern_request = $_GET['487m8c5489c547m84']; //spotify login button pattern
$amazon_skip_button_pattern_request = $_GET['4789475489c5m7']; //spotify login button pattern
$amazon_login_error_pattern_request = $_GET['84c7m857849c5']; //spotify login button pattern

//Deezer Requests
$deezer_signup_button_pattern_request = $_GET['5u5zum8uc47c5m']; //spotify login username pattern
$deezer_login_username_pattern_request = $_GET['98658905698596']; //spotify login username pattern
$deezer_login_password_pattern_request = $_GET['5698748c57m48c']; //spotify login password pattern
$deezer_login_button_pattern_request = $_GET['p9695695869589v']; //spotify login button pattern
$deezer_play_button_pattern_request = $_GET['568v7m56875v8675m8v']; //spotify login button pattern
$deezer_skip_button_pattern_request = $_GET['908786898796b6b76']; //spotify login button pattern
$deezer_login_error_pattern_request = $_GET['6758976v89576m85']; //spotify login button pattern

$tables = "autobot_users";

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
        
        if ($token_provided == $row['token']) //if token correct
        {
            if($row['banned_reason'] != "")
            {
                echo "banned"; // user banned
            }
            
            //Spotify Patterns   
            if($spotify_login_username_pattern_request == "8937m83x74893")
            {
                echo "zdoQRwn3TRuWCRYNbM07fSF/4and/FmQonnHoVobw+GgO6srUsz0jjRJTNimxMCQkP8VlUCDA2nlQ43ox6txDiNtgbFhmVN9IVSUblBgOwJuDBtHMagqSgP6dm23br7H"; //spotify login username pattern
            }
            
            if($spotify_login_password_pattern_request == "756nx46453646")
            {
                echo "QjGz6pLgePVm/ssjhY4vL0JOw7MoX31+KvEGfIQRRSj27v22EgIPnDLZgXHast9oDzQrrVqM1JvlPn4WHIjHd4FRTs4JU7VXhQP9LyNwo0wxqYZfZPv35PBOMTQrFq9m"; //spotify login password pattern
            }
            
            if($spotify_login_button_pattern_request == "783x7834x3x43")
            {
                echo "8XvJg0BozkQLLHA6nkTYn3wwfbHBxElb3uGibmtM9/S0VAmJaBPvVNCTloxo3cmxDbrUceZc2jI5+KIKhWghmMhi95j73beugYj1aA2ycTIGGTdCJONhMmrOWqpeYq/U"; //spotify login button pattern
            }
            
            if($spotify_login_verify_pattern_request == "4834783748344")
            {
                echo "zq/uDIHILqAj8jbX6q6cDu0mGh44lZTB4b2gjiLiMtnLukD8kN2XYx3dKFHJdb7Qq+2VpMAVLCWYCcTEs3YCWOcLAjvZuG5din3FH/BlhB9AgXKkw6V7C6LUuHRClNjvRphoUdNKJBkwGds+XZK3Es7n9UTifc+ciG9FNHZnYu4="; //spotify login button pattern
            }
            
            if($spotify_play_button_pattern_request == "n753c6n7c5c536")
            {
                echo "OXKikKLMzpraCd+2UOfiuRKBr6L8Zm73JHQeXRXZI7i8gU7b0yLoVW97/u2VSEv63dHAmtX9I7Kk/HwIrKlAJAfId9ny1a9jkGBAGdvmVlFPeRjn3DqWNuhjV8LgIKoS"; //spotify login button pattern
            }
            
            if($spotify_skip_button_pattern_request == "x473x9783x783")
            {
                echo "enunxjGKQNNpRfuJIS0/YNvvTJ52BUI3IrCwSyNq7Gmtwkv/qu0Q1J7QN8r54E0nRr0q67fwVjncTlZJaBtb62WGpbxB+lPm8qRZl2fdkXPh5Xd1JdBMaKyDK5IkmTVs0F/9LA5d/aJL4c/D+OtE1SSCaIpRBqCoq8OcJovM3x4="; //spotify login button pattern
            }
            
            if($spotify_login_error_pattern_request == "x38x482934833")
            {
                echo "MARFt2zwAmZzf9fHWVJNH+b3ISeyOF0zW9jR+jQFq4z2Lz6bQAQNFbRRjoRrfii+mv7eL/jUuQKAYJ2dF7iSNROYz85G5qpQJGcL/anu66aU1MdO0F0Haxn0QinkdBMtJOXH25Fm0IhXzpcoQHDQEpJYb9nbfSOOGmoe7FjMR0U="; //spotify login button pattern
            }
            
            //Amazon Patterns
            if($amazon_signup_button_pattern_request == "7894n63576n756n7")
            {
                echo "GY47O4e6z+Cdj5qy6rLCdU0YFhE7LQ5ql7X55gicUWZ4tRwgj3SYyBBXCMzWNXwXzabIfbvp4RcJ6pEDQHLvnHFhHO6FL/6RKWkRAr/2FPC9Sb4w9EXX2PqJf9mc341v"; //Amazon login username pattern
            }
            
            if($amazon_login_username_pattern_request == "847cm4758948c")
            {
                echo "s6sKZOzEdSC19B3OyrobKHmf9F98cF7xPax1Ldk9Hqsal3kbIYWp/mx5wFymMtdV5vIS/DqV7S4z2YDWbJtdzPreMlSm+oGg+VS6AYCHs5XzS2Mt6Z/GIkvCxUUW1FKY"; //Amazon login username pattern
            }
            
            if($amazon_login_password_pattern_request == "cm8475m8947c584c")
            {
                echo "CtNJDfZtXm8q1c93JBX6bYDf28orY5iDhscqDN06gsnq/5lc6Z0NjRW+/oFbonloRK0HYQaTlzS13uACqR/52/JgN5e1GeGZxab2zBWN+dXtVauYVzFVJ+R9E9Vlh82D"; //Amazon login password pattern
            }
            
            if($amazon_login_button_pattern_request == "78c47m854c7m5")
            {
                echo "4tSyS+Z0DpZPORCXYG+jv+sRhNEO6S71kdKg/U9pOMp3LgtaO5NdeSnH0AmgwDsVf5rghY6vcb3H7IYSOZ6ZBTWfXPhFYwnTvicmg+OCd8TMFV1O1tXtDA41TJDi1DRa"; //Amazon login button pattern
            }
            
            if($amazon_play_button_pattern_request == "487m8c5489c547m84")
            {
                echo "c9xN4G9EFVLkanQwbyU8Vfz0vErxwKZT/T2HByaBzYqGdyhIM69rh1JJZCtXT56CQ0i2mnSSm5LOIZ/Kh4gE4vvexW0FazbJxTtynxzBr+bJ4POTx2h/6Q/gqXqS+4nG5TNRVCIFHVb10bdCHUYYgWFSelCO8l0d304wNOrfANc="; //Amazon login button pattern
            }
            
            if($amazon_skip_button_pattern_request == "4789475489c5m7")
            {
                echo "8n4n1Pl6nCCjkl0JB1tpGpHq2lDScDqh3rfKS7sO8kLWo+s+n2uZQH4aURRaCjqHiFJ3OFH2ezRUK5jFtoBPaBCaUICkrxZgn7+nCE2vFhv+qCegxYqWVC9CzLlQlZY7T3KuNE0RFq20s5sI7/NAecY08fRWU0SWxcbPG41ac1A="; //Amazon login button pattern
            }
            
            //Deezer Patterns
            if($deezer_signup_button_pattern_request == "5u5zum8uc47c5m")
            {
                echo "tQInHyCmZJkHOx8s2VNRzzhj2vgal83Vm7XJ4XSqpzbxWTxLizVZPBPTSHexdpe9hbvgkw7bfejA1Pll4NabyWIASQQkFH/TJojY4Yjr8T2ZD4ina+Vk4M5nxNWvMyGp"; //deezer signin pattern
            }
            
            if($deezer_login_username_pattern_request == "98658905698596")
            {
                echo "/HjosqvqjBEhWKyvupcd8gQSCcKnm42JSdr4UdK1yb/Y8N7iqRaBE0oXov6h2RNaaAnF+EVyvVXNU/6GmX0iqGHOfkF8dYs5ugNMIL0iLQo4AzKJWMKK/l8Trg+ECNzu"; //deezer login username pattern
            }
            
            if($deezer_login_password_pattern_request == "5698748c57m48c")
            {
                echo "iLD1NTpuhPSQ4jy6Cp7zSPDeIgNl75hEiXenyVC1gdn0n3xPKZGo91DE9N7FTgCimHWGiSdZsjn3I6Z3lc0vUm5TOIAcCEg2Xl4JQLvy4/GdsHvG6KZ/C1fqOIVlqWEt"; //deezer login password pattern
            }
            
            if($deezer_login_button_pattern_request == "p9695695869589v")
            {
                echo "1rISNNX99+sVmooXOvl/eKs1SZl15iidP6SNa/Bp4VlfFQ7r5KmKjI6t7o/Nj1OmHhP8jadO2kYAh/ZqjCT3NZRSmuGjK81md/wN/7AVhfGieM8g1aWUxGRYX4396+/3"; //deezer login button pattern
            }
            
            if($deezer_play_button_pattern_request == "568v7m56875v8675m8v")
            {
                echo "a1eIsP3Dt4eQNt1daogft3xcN4krOlK3fhcrt6hy5mCvP2VzY6FfCAAkR/qI/ogtRw8s7rV81MsBkaIFCOQWzDFC2KDtCAIPtP5lsaBvWtcFPyIl0bvS1J+8QverSq9zmaS4K76FC/NO1upmi29t4PkEN9J8mS+tIejzSYba52QCn/gtGFH8HnXndmUyRoqx/Zd+BpaeodIn4oPV4cia3A=="; //deezer login button pattern
            }
            
            if($deezer_skip_button_pattern_request == "908786898796b6b76")
            {
                echo "V15Xi2VTPUAxnv2l4n7D9AXZvN+zDdKjY2DQO8p6ByLTe5PxrXFVNrxZJmo84dBBGtpgpVNaZW/DfmRdoSlyMDsym3k2/x5do+Jv7TLTi9fdEPa6cz/4DhFHHuH5O4xiFj1wiY2fxEUMu+k6nertadnGh5Dw5tl7gio9qSTgFUY="; //deezer login button pattern
            }
        }
        else
        {
            echo "token_incorrect"; //token incorrect
        }
    }
}  
?>